import 'package:flutter/material.dart';
import 'package:frontend/survey.dart';
import 'package:frontend/sleepButton.dart';

class LoadingPage extends StatefulWidget {
  const LoadingPage({Key? key, required this.title}) : super(key: key);
  final String title;

  @override
  State<LoadingPage> createState() => LoadingPageState();
}

class LoadingPageState extends State<LoadingPage> {
  String _credentials = "";

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(body: Center(child: Text("loading")));
  }
}
