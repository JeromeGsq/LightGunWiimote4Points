import 'dart:io';

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

  List<Offset> offset = [
    Offset(100, 0),
    Offset(0, 0),
    Offset(0, 100),
    Offset(100, 100),
  ];

  @override
  Future<void> load() async {
    // await Future.delayed(const Duration(seconds: 1));
    connected = true;
    file = File('C:\\Users\\PC\\Desktop\\position.txt');
    run();
  }

  Future<void> run() async {
    while (true) {
      try {
        final data = await file.readAsLines();
        await Future.delayed(const Duration(milliseconds: 16));
        offset.clear();

        for (var line in data) {
          final parts = line.split(":");
          offset.add(
            Offset(
              (0.5 - double.parse(parts[0].replaceAll(",", "."))) * 250,
              (0.5 - double.parse(parts[1].replaceAll(",", "."))) * 250,
            ),
          );
        }
        notifyListeners();
      } catch (_) {
        offset = [
          Offset(100, 0),
          Offset(0, 0),
          Offset(0, 100),
          Offset(100, 100),
        ];
      }
    }
  }
}
