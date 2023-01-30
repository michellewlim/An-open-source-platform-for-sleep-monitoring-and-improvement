import 'package:flutter/material.dart';
import 'package:frontend/survey.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SleepButtonPage extends StatefulWidget {
  const SleepButtonPage({Key? key}) : super(key: key);

  @override
  State<SleepButtonPage> createState() => SleepButtonPageState();
}

class SleepButtonPageState extends State<SleepButtonPage> {
  void handleClick() async {
    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const SurveyPage(
        title: "survey page",
      );
    }));

    final prefs = await SharedPreferences.getInstance();
    prefs.setBool('asleep', true);
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
