import 'package:flutter/material.dart';
import 'package:frontend/survey.dart';
import 'dart:developer';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:pkce/pkce.dart';
import 'package:fitbitter/fitbitter.dart';

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
  String _username = "";

  @override
  void initState() {
    super.initState();
    _loadUsername();
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
    log('fitbitId: $fitbitId');
    log('nestId: $nestId');

    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const SurveyPage(title: 'SurveyPage');
    }));

    final pkcePair = PkcePair.generate();
    String authUrl =
        "https://www.fitbit.com/oauth2/authorize?client_id=2392N5&response_type=code&code_challenge=${pkcePair.codeChallenge}&code_challenge_method=S256&scope=activity%20heartrate%20location%20nutrition%20oxygen_saturation%20profile%20respiratory_rate%20settings%20sleep%20social%20temperature%20weight";
    FitbitCredentials? fitbitCredentials = await FitbitConnector.authorize(
        clientID: "2392DX",
        clientSecret: "5608120c565c67f8abd15a10d07e80b3",
        redirectUri: "http://localhost:61405/#/",
        callbackUrlScheme: "https");
    log(fitbitCredentials.toString());
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
