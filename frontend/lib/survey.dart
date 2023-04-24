import 'package:flutter/material.dart';
import 'package:frontend/sleepButton.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:developer';
import 'package:frontend/main.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:io';

class SurveyPage extends StatefulWidget {
  const SurveyPage({Key? key, required this.title}) : super(key: key);
  final String title;
//ask for username and nestid
//send these to the back end *for now console.log them?

  @override
  State<SurveyPage> createState() => SurveyPageState();
}

class SurveyPageState extends State<SurveyPage> {
  int _username = 0;
  String? _sleepRating = "";
  String? _disturbance = "";
  String? _disturbanceDescription = "";

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

  Future<void> _removeUsername() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('username');
    await prefs.remove('credentials');
    setState(() {
      _username = 0;
    });
  }

  Future<void> _setUsername(Text username) async {
    final prefs = await SharedPreferences.getInstance();
    prefs.setInt('username', int.parse(username.toString()));
  }

  void handleSubmit() async {
    if (_sleepRating == "") {
      log("Please rate your sleep");
    } else if (_disturbance == "") {
      log("Please specify if you encountered any disturbances during your sleep");
    } else if (_disturbance == "yes" && _disturbanceDescription == "") {
      log("Please descripbe the disturbance encountered during your sleep");
    } else {
      log('$_sleepRating, $_disturbance, $_disturbanceDescription');
    }

    HttpOverrides.global = MyHttpOverrides();
    var headers = {'Content-Type': 'application/json'};
    var wakeUserRequest = http.Request(
        'PUT',
        Uri.parse(
            'https://www.an-open-source-platform-for-sleep-monitoring-and-i.com/Manager/Wake'));
    wakeUserRequest.body = json.encode({"userID": _username});
    wakeUserRequest.headers.addAll(headers);

    http.StreamedResponse wakeUserResponse = await wakeUserRequest.send();
    log(wakeUserResponse.statusCode.toString());

    Navigator.push(context, MaterialPageRoute(builder: (context) {
      return const SleepButtonPage(
        title: 'sleep button page',
      );
    }));

    final prefs = await SharedPreferences.getInstance();
    prefs.setBool('asleep', false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          title: const Text('survey page'),
        ),
        body: Column(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                mainAxisSize: MainAxisSize.min,
                children: <TextButton>[
                  TextButton(
                      onPressed: () => {_removeUsername()},
                      child: const Text('Delete')),
                  TextButton(
                      onPressed: () => {_setUsername(const Text('michelle'))},
                      child: const Text('Submit')),
                ]),
            Column(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              mainAxisSize: MainAxisSize.min,
              children: <Widget>[
                const Text("Please rate your subjective sleep quality"),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: <Widget>[
                    SizedBox(
                        width: 100,
                        height: 100,
                        child: ListTile(
                          title: const Text('1'),
                          leading: Radio(
                            value: "1",
                            groupValue: _sleepRating,
                            onChanged: (String? value) {
                              setState(() {
                                _sleepRating = value;
                              });
                            },
                          ),
                        )),
                    SizedBox(
                        width: 100,
                        height: 100,
                        child: ListTile(
                          title: const Text('2'),
                          leading: Radio(
                            value: "2",
                            groupValue: _sleepRating,
                            onChanged: (String? value) {
                              setState(() {
                                _sleepRating = value;
                              });
                            },
                          ),
                        )),
                    SizedBox(
                        width: 100,
                        height: 100,
                        child: ListTile(
                          title: const Text('3'),
                          leading: Radio(
                            value: "3",
                            groupValue: _sleepRating,
                            onChanged: (String? value) {
                              setState(() {
                                _sleepRating = value;
                              });
                            },
                          ),
                        )),
                    SizedBox(
                        width: 100,
                        height: 100,
                        child: ListTile(
                          title: const Text('4'),
                          leading: Radio(
                            value: "4",
                            groupValue: _sleepRating,
                            onChanged: (String? value) {
                              setState(() {
                                _sleepRating = value;
                              });
                            },
                          ),
                        )),
                    SizedBox(
                        width: 100,
                        height: 100,
                        child: ListTile(
                          title: const Text('5'),
                          leading: Radio(
                            value: "5",
                            groupValue: _sleepRating,
                            onChanged: (String? value) {
                              setState(() {
                                _sleepRating = value;
                              });
                            },
                          ),
                        )),
                  ],
                )
              ],
            ),
            Column(
                //mainAxisAlignment: MainAxisAlignment.spaceBetween,
                mainAxisSize: MainAxisSize.min,
                children: <Widget>[
                  const Text(
                      "Did you experience any disturbances during the night?"),
                  Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: <Widget>[
                        SizedBox(
                            width: 200,
                            height: 100,
                            child: ListTile(
                              title: const Text('yes'),
                              leading: Radio(
                                value: "yes",
                                groupValue: _disturbance,
                                onChanged: (String? value) {
                                  setState(() {
                                    _disturbance = value;
                                  });
                                },
                              ),
                            )),
                        SizedBox(
                            width: 200,
                            height: 100,
                            child: ListTile(
                              title: const Text('no'),
                              leading: Radio(
                                value: "no",
                                groupValue: _disturbance,
                                onChanged: (String? value) {
                                  setState(() {
                                    _disturbance = value;
                                  });
                                },
                              ),
                            ))
                      ]),
                ]),
            if (_disturbance == "yes")
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 30),
                child: TextFormField(
                  decoration: const InputDecoration(
                      border: UnderlineInputBorder(),
                      labelText: "What was the disturbance"),
                  onChanged: (text) {
                    _disturbanceDescription = text;
                  },
                ),
              ),
            Text(_username.toString()),
            TextButton(
                onPressed: () => {handleSubmit()}, child: const Text("submit"))
          ],
        ));
  }
}
