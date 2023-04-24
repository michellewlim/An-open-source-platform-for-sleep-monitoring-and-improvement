import 'package:flutter/material.dart';
import 'package:frontend/survey.dart';
import 'package:frontend/main.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'dart:developer';
import 'dart:convert';
import 'dart:io';

class SleepButtonPage extends StatefulWidget {
  const SleepButtonPage({Key? key, required this.title}) : super(key: key);
  final String title;

  @override
  State<SleepButtonPage> createState() => SleepButtonPageState();
}

class SleepButtonPageState extends State<SleepButtonPage> {
  int _username = 0;

  @override
  void initState() {
    super.initState();
    _loadUsername();
  }

  Future<void> _loadUsername() async {
    final prefs = await SharedPreferences.getInstance();
    setState(() {
      _username = (prefs.getInt('username') ?? 0);
    });
  }

  void handleClick() async {
    HttpOverrides.global = MyHttpOverrides();
    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const SurveyPage(
        title: "survey page",
      );
    }));

    final prefs = await SharedPreferences.getInstance();
    prefs.setBool('asleep', true);

    var headers = {'Content-Type': 'application/json'};
    var sleepUserRequest = http.Request(
        'PUT',
        Uri.parse(
            'https://www.an-open-source-platform-for-sleep-monitoring-and-i.com/Manager/Sleep'));
    sleepUserRequest.body = json.encode({"userID": _username});
    sleepUserRequest.headers.addAll(headers);

    http.StreamedResponse sleepUserResponse = await sleepUserRequest.send();
    log(sleepUserResponse.statusCode.toString());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        body: Center(
      child: ElevatedButton(
          onPressed: () => {handleClick()},
          child: Text("Going to Sleep",
              style: TextStyle(fontFamily: 'RaleWay', fontSize: 30)),
          style: ElevatedButton.styleFrom(
              shape: CircleBorder(),
              padding: const EdgeInsets.all(100),
              backgroundColor: Colors.blue,
              foregroundColor: Colors.black)),
    ));
  }
}
