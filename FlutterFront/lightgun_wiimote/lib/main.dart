import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:lightgun_wiimote/modules/loading/view.dart';

void main() {
  runApp(Run());
}

class Run extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Provider(
      create: (_) => RouteObserver<PageRoute>(),
      child: App(),
    );
  }
}

class App extends StatelessWidget {
  const App({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Lightgun Front',
      debugShowCheckedModeBanner: false,
      navigatorObservers: [
        context.watch<RouteObserver<PageRoute>>(),
      ],
      theme: ThemeData(
        primarySwatch: Colors.blue,
        textTheme: TextTheme(
          bodyText2: TextStyle(fontSize: 22),
        ),
      ),
      home: LoadingView(),
    );
  }
}
