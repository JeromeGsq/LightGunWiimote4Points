import 'package:flutter/material.dart';

class LabeledCheckbox extends StatelessWidget {
  const LabeledCheckbox({
    Key? key,
    @required this.label,
    @required this.value,
    this.onChanged,
  }) : super(key: key);

  final String? label;
  final bool? value;
  final ValueChanged<bool?>? onChanged;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(8),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Checkbox(
            value: value,
            onChanged: onChanged,
          ),
          Text(label ?? ''),
        ],
      ),
    );
  }
}
