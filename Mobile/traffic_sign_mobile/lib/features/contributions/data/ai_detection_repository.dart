import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/config/app_config.dart';
import '../../../core/storage/token_storage.dart';
import 'models/ai_detection_result.dart';

class AIDetectionRepository {
  AIDetectionRepository(this._config, this._tokenStorage);

  final AppConfig _config;
  final TokenStorage _tokenStorage;

  Future<AIDetectionResult> detectSigns(File imageFile) async {
    final formData = FormData.fromMap({
      'image': await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.path.split('/').last,
      ),
    });

    // Use Dio directly for multipart upload
    final dio = Dio(
      BaseOptions(
        baseUrl: _config.apiBaseUrl,
        connectTimeout: const Duration(seconds: 60), // AI processing can take time
        receiveTimeout: const Duration(seconds: 60),
        headers: {'Content-Type': 'multipart/form-data'},
      ),
    );

    // Add auth token
    final token = await _tokenStorage.readAccessToken();
    if (token != null && token.isNotEmpty) {
      dio.options.headers['Authorization'] = 'Bearer $token';
    }

    try {
      final response = await dio.post<Map<String, dynamic>>(
        '/api/ai/detect',
        data: formData,
      );

      final data = response.data?['data'] as Map<String, dynamic>?;
      if (data == null) {
        throw Exception('Invalid response from AI service');
      }

      return AIDetectionResult.fromJson(data);
    } finally {
      dio.close();
    }
  }
}

final aiDetectionRepositoryProvider = Provider<AIDetectionRepository>((ref) {
  final config = ref.watch(appConfigProvider);
  final tokenStorage = ref.watch(tokenStorageProvider);
  return AIDetectionRepository(config, tokenStorage);
});

