import 'package:flutter/material.dart';
import 'package:frontend/home.dart';
import 'dart:io';

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

class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}
