import 'dart:ffi';

import 'package:flutter/material.dart';
import 'package:frontend/sleepButton.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:frontend/signin.dart';
import 'package:frontend/survey.dart';
//import 'package:flutter_application_1/api_service.dart';

class Home extends StatefulWidget {
  const Home({Key? key}) : super(key: key);

  @override
  State<Home> createState() => HomeState();
}

class HomeState extends State<Home> {
  String _username = "";
  late bool _asleep = false;

  @override
  void initState() {
    super.initState();
    _loadUsername();
    _loadAsleep();
  }

  Future<void> _loadUsername() async {
    final prefs = await SharedPreferences.getInstance();
    setState(() {
      _username = (prefs.getString('username') ?? "");
    });
  }

  Future<void> _loadAsleep() async {
    final prefs = await SharedPreferences.getInstance();
    setState(() {
      _asleep = (prefs.getBool('asleep') ?? false);
    });
  }

  Widget checkForUsername() {
    if (_username == "") {
      return SignInPage(title: 'sign in page');
    } else if (_asleep == false) {
      return SleepButtonPage();
    } else {
      return SurveyPage(title: 'survey page');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: checkForUsername(),
    );
  }
}
