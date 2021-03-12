import 'package:flutter/material.dart';
import 'package:lightgun_wiimote/widgets/app_labeled_checkbox.dart';

class WiimoteWidget extends StatelessWidget {
  const WiimoteWidget({
    Key? key,
    this.label,
  }) : super(key: key);

  final String? label;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label ?? '',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              LabeledCheckbox(label: 'Up', value: false),
              LabeledCheckbox(label: 'Right', value: false),
              LabeledCheckbox(label: 'Down', value: false),
              LabeledCheckbox(label: 'Left', value: false),
              SizedBox(height: 10),
              LabeledCheckbox(label: 'A', value: false),
              LabeledCheckbox(label: 'B', value: false),
              SizedBox(height: 10),
              LabeledCheckbox(label: '-', value: false),
              LabeledCheckbox(label: 'Home', value: false),
              LabeledCheckbox(label: '+', value: false),
              SizedBox(height: 10),
              LabeledCheckbox(label: '1', value: false),
              LabeledCheckbox(label: '2', value: false),
            ],
          ),
        ),
      ),
    );
  }
}
