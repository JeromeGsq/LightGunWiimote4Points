import 'dart:io';
import 'dart:async';

import 'package:flutter/material.dart';
import 'package:lightgun_wiimote/core/widgets/notifier.dart';

class LoadingNotifier extends BaseNotifier {
  late File file;

  bool _connected = false;
  bool get connected => _connected;
  set connected(bool connected) {
    if (_connected != connected) {
      _connected = connected;
      notifyListeners();
    }
  }

  late Socket socket;

  List<Offset> offset = [
    Offset(100, 0),
    Offset(0, 0),
    Offset(0, 100),
    Offset(100, 100),
  ];

  @override
  Future<void> load() async {
    connected = true;
    run();
  }

  void run() {
    Socket.connect("localhost", 11111).then((Socket sock) {
      socket = sock;
      socket.listen(
        dataHandler,
        onError: errorHandler,
        onDone: doneHandler,
        cancelOnError: false,
      );
    }).catchError((AsyncError e) {
      print("Unable to connect: $e");
    });
  }

  Future<void> dataHandler(result) async {
    await Future.delayed(const Duration(milliseconds: 16));

    List<String> data = String.fromCharCodes(result).trim().split("\n");
    print(data);

    try {
      for (var i = 0; i <= data.length - 1; i++) {
        final parts = data[i].split(":");
        offset[i] = Offset(
          -(0.5 - double.parse(parts[0].replaceAll(",", "."))) * 250,
          (0.5 - double.parse(parts[1].replaceAll(",", "."))) * 250,
        );
      }
    } catch (e) {
      print(e);
    }
    notifyListeners();
  }

  void errorHandler(error, StackTrace trace) {
    print(error);
  }

  void doneHandler() {
    socket.destroy();
  }
}
