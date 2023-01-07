import 'package:flutter/material.dart';
import 'package:frontend/home.dart';

void main() => runApp(const MyApp());

//check whether there is a username already and if so --> display the survey
//if not --> display the sign up page.

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      title: 'Material App',
      home: Home(),
    );
  }
}
