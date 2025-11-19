import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../app/app_router.dart';
import '../app/app_theme.dart';

class TrafficSignApp extends ConsumerWidget {
  const TrafficSignApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(appRouterProvider);

    return MaterialApp.router(
      title: 'Traffic Sign',
      theme: AppTheme.light,
      routerConfig: router,
    );
  }
}
