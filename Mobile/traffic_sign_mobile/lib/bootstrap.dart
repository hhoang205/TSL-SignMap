import 'dart:async';

import 'package:flutter/widgets.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'app/app.dart';
import 'features/auth/application/auth_controller.dart';
import 'core/storage/token_storage.dart';

/// Bootstraps the Flutter application by:
/// 1. Ensuring Flutter bindings are initialized.
/// 2. Loading environment variables from `.env`.
/// 3. Restoring the persisted auth session before rendering the UI.
Future<void> bootstrap() async {
  WidgetsFlutterBinding.ensureInitialized();

  await dotenv.load(
    fileName: const String.fromEnvironment('ENV_FILE', defaultValue: '.env'),
  );

  final tokenStorage = await initTokenStorage();
  final container = ProviderContainer(
    overrides: [tokenStorageProvider.overrideWithValue(tokenStorage)],
  );
  await container.read(authControllerProvider.notifier).restoreSession();

  runApp(
    UncontrolledProviderScope(
      container: container,
      child: const TrafficSignApp(),
    ),
  );
}
