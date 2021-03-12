import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:lightgun_wiimote/modules/loading/notifier.dart';

class LoadingView extends StatelessWidget {
  const LoadingView({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<LoadingNotifier>(
      create: (_) => LoadingNotifier()..load(),
      child: View(),
    );
  }
}

class View extends StatelessWidget {
  const View({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return context.watch<LoadingNotifier>().connected
        ? Body()
        : LoadingWidget();
  }
}

class LoadingWidget extends StatelessWidget {
  const LoadingWidget({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.blueGrey,
      body: Center(
        child: Text('Connecting...'),
      ),
    );
  }
}

class Body extends StatelessWidget {
  const Body({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<LoadingNotifier>();
    return Scaffold(
      backgroundColor: Colors.blueGrey,
      appBar: AppBar(title: Center(child: Text('Lightgun UI'))),
      body: Center(
        child: Container(
          height: 500,
          width: 500,
          color: Colors.black,
          child: Builder(
            builder: (context) {
              if (vm.offset.length >= 4) {
                return Stack(
                  children: [
                    Point(offset: vm.offset[0], color: Colors.red),
                    Point(offset: vm.offset[1], color: Colors.green),
                    Point(offset: vm.offset[2], color: Colors.blue),
                    Point(offset: vm.offset[3], color: Colors.cyan),
                  ],
                );
              } else {
                return SizedBox();
              }
            },
          ),
        ),
      ),
    );
  }
}

class Point extends StatelessWidget {
  const Point({
    Key? key,
    @required this.offset,
    @required this.color,
  }) : super(key: key);

  final Offset? offset;
  final Color? color;

  @override
  Widget build(BuildContext context) {
    return Transform.translate(
      offset: offset ?? Offset(0, 0),
      child: Transform.scale(
        scale: 0.01,
        child: Container(
          color: color ?? Colors.red,
        ),
      ),
    );
  }
}
