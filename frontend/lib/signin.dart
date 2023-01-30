import 'dart:async';

import 'package:flutter/material.dart';
import 'package:frontend/sleepButton.dart';
import 'package:frontend/loading.dart';
import 'dart:developer';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:fitbitter/fitbitter.dart';
import 'package:app_links/app_links.dart';

class SignInPage extends StatefulWidget {
  const SignInPage({Key? key, required this.title}) : super(key: key);
  final String title;

//ask for username and nestid
//send these to the back end *for now console.log them?
  @override
  State<SignInPage> createState() => SignInPageState();
}

class SignInPageState extends State<SignInPage> {
  final usernameController = TextEditingController();
  final fitBitIdController = TextEditingController();
  final nestIdController = TextEditingController();
  late AppLinks _appLinks;
  String _username = "";
  String? initialLink = "";
  StreamSubscription<Uri>? _linkSubscription;
  final _navigatorKey = GlobalKey<NavigatorState>();

  @override
  void initState() {
    super.initState();
    _loadUsername();
    initDeepLinks();
  }

  @override
  void dispose() {
    _linkSubscription?.cancel();

    super.dispose();
  }

  Future<void> initDeepLinks() async {
    _appLinks = AppLinks();
    final appLink = await _appLinks.getInitialAppLink();

    if (appLink != null) {
      log('getInitialAppLink: $appLink');
    }

    // Handle link when app is in warm state (front or background)
    _linkSubscription = _appLinks.uriLinkStream.listen((uri) {
      log('onAppLink: $uri');
      openAppLink(uri);
    });
  }

  void openAppLink(Uri uri) {
    _navigatorKey.currentState?.pushNamed(uri.fragment);
  }

  Future<void> _loadUsername() async {
    final prefs = await SharedPreferences.getInstance();
    setState(() {
      _username = (prefs.getString('username') ?? "");
    });
  }

  Future<void> _setUsername(Text username) async {
    final prefs = await SharedPreferences.getInstance();
    prefs.setString('username', username.toString());
  }

  void handleSubmit(Text username, Text fitbitId, Text nestId) async {
    _setUsername(username);

    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const LoadingPage();
    }));

    final prefs = await SharedPreferences.getInstance();

    FitbitCredentials? fitbitCredentials = await FitbitConnector.authorize(
        clientID: "2392DX",
        clientSecret: "5608120c565c67f8abd15a10d07e80b3",
        redirectUri: "dreamtemp://auth",
        callbackUrlScheme: "dreamtemp");

    log(fitbitCredentials.toString());

    prefs.setString('credentials', fitbitCredentials.toString());

    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const SleepButtonPage();
    }));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          title: const Text("Sign In Page"),
        ),
        body: Column(
          children: <Widget>[
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
              child: TextFormField(
                decoration: const InputDecoration(
                    border: UnderlineInputBorder(),
                    labelText: "Enter your username"),
                controller: usernameController,
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
              child: TextFormField(
                decoration: const InputDecoration(
                    border: UnderlineInputBorder(),
                    labelText: "Enter your nest thermostat id"),
                controller: fitBitIdController,
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
              child: TextFormField(
                decoration: const InputDecoration(
                    border: UnderlineInputBorder(),
                    labelText: "Enter your fitbit id"),
                controller: nestIdController,
              ),
            ),
            Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
                child: TextButton(
                    onPressed: () => {
                          handleSubmit(
                              Text(usernameController.text),
                              Text(fitBitIdController.text),
                              Text(nestIdController.text))
                        },
                    child: const Text("submit"))),
            Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
                child: Text(_username)),
          ],
        ));
  }
}
