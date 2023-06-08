import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:frontend/sleepButton.dart';
import 'package:frontend/loading.dart';
import 'package:frontend/main.dart';
import 'dart:developer';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:fitbitter/fitbitter.dart';
import 'package:app_links/app_links.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_web_auth/flutter_web_auth.dart';

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
  int _username = 0;
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
      _username = (prefs.getInt('username') ?? 0);
    });
  }

  Future<void> _setUsername(int username) async {
    final prefs = await SharedPreferences.getInstance();
    prefs.setInt('username', username);
  }

  void handleSubmit(int username, Text fitbitId, Text nestId) async {
    HttpOverrides.global = MyHttpOverrides();
    //store username on local disk
    _setUsername(username);

    //change page to loading page
    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return LoadingPage(title: "loading page");
    }));

    FitbitCredentials? fitbitCredentials = await FitbitConnector.authorize(
        clientID: "2392DX",
        clientSecret: "5608120c565c67f8abd15a10d07e80b3",
        redirectUri: "dreamtemp://auth",
        callbackUrlScheme: "dreamtemp");

    log(fitbitCredentials.toString());
    log(fitbitCredentials!.userID);

    //store credentials on local disk
    final prefs = await SharedPreferences.getInstance();
    prefs.setString('credentials', fitbitCredentials.toString());

    //nest thermostat auth

    final url = Uri.https('accounts.google.com', '/o/oauth2/v2/auth', {
      'response_type': 'code',
      'client_id':
          '251426184244-1cmn8e95c18c53aaol65s1rjgqruk93v.apps.googleusercontent.com',
      'redirect_uri': 'com.example.frontend:/auth',
      'scope': 'https://www.googleapis.com/auth/sdm.service',
    });

    // Present the dialog to the user
    final nestAuthResult = await FlutterWebAuth.authenticate(
        url: url.toString(), callbackUrlScheme: 'com.example.frontend');

    final code = Uri.parse(nestAuthResult).queryParameters['code'];
    final uri = Uri.https('www.googleapis.com', 'oauth2/v4/token');
    final nestAuthResponse = await http.post(uri, body: {
      'client_id':
          '251426184244-1cmn8e95c18c53aaol65s1rjgqruk93v.apps.googleusercontent.com',
      'redirect_uri': 'com.example.frontend:/auth',
      'grant_type': 'authorization_code',
      'code': code,
    });
    log(jsonDecode(nestAuthResponse.body).toString());

    var headers = {'Content-Type': 'application/json'};

    var addUserRequest = await http.Request(
        'POST',
        Uri.parse(
            'https://www.an-open-source-platform-for-sleep-monitoring-and-i.com/UserData/AddUser'));
    addUserRequest.body = json.encode({
      'userId': username,
      'age': '18',
      'sex': 'f',
      'nestID': nestId.toString(),
      'fitbitID': fitbitId.toString()
    });
    addUserRequest.headers.addAll(headers);
    http.StreamedResponse addUserResponse = await addUserRequest.send();

    var linkFitbitRequest = await http.Request(
        'PUT',
        Uri.parse(
            'https://www.an-open-source-platform-for-sleep-monitoring-and-i.com/UserData/LinkFitbit'));
    linkFitbitRequest.body = json.encode({
      'userId': username,
      'fitbitID': fitbitCredentials.userID,
      'accessToken': fitbitCredentials.fitbitAccessToken,
      'refreshToken': fitbitCredentials.fitbitRefreshToken,
    });
    linkFitbitRequest.headers.addAll(headers);
    http.StreamedResponse linkFitbitResponse = await linkFitbitRequest.send();

    String nestAccessToken =
        jsonDecode(nestAuthResponse.body)["access_token"] as String;
    String nestRefreshToken =
        jsonDecode(nestAuthResponse.body)["refresh_token"] as String;
    int nestExpireIn = jsonDecode(nestAuthResponse.body)["expires_in"] as int;

    var linkNestRequest = await http.Request(
        'PUT',
        Uri.parse(
            'https://www.an-open-source-platform-for-sleep-monitoring-and-i.com/UserData/LinkNest'));
    linkNestRequest.body = json.encode({
      'userID': username,
      "accessToken": nestAccessToken,
      "refreshToken": nestRefreshToken,
      "expires_in": nestExpireIn
    });
    linkNestRequest.headers.addAll(headers);
    http.StreamedResponse linkNestResponse = await linkNestRequest.send();
    log(linkNestResponse.statusCode.toString());

    //change page to sleep button page
    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return SleepButtonPage(title: "sleep button page");
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
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                    border: UnderlineInputBorder(),
                    labelText: "Enter your user id"),
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
                              int.parse(usernameController.text),
                              Text(fitBitIdController.text),
                              Text(nestIdController.text))
                        },
                    child: const Text("submit"))),
            Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
                child: Text(_username.toString())),
          ],
        ));
  }
}
