import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'bootstrap.dart';
import 'firebase_options.dart';

import '../features/auth/presentation/user_screen.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Khởi tạo Firebase
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  
  await bootstrap();
}