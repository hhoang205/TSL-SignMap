import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/config/app_config.dart';
import '../../../core/storage/token_storage.dart';

class ImageUploadRepository {
  ImageUploadRepository(this._config, this._tokenStorage);

  final AppConfig _config;
  final TokenStorage _tokenStorage;

  Future<String> uploadImage(File imageFile) async {
    final formData = FormData.fromMap({
      'image': await MultipartFile.fromFile(
        imageFile.path,
        filename: imageFile.path.split('/').last,
      ),
    });

    final dio = Dio(
      BaseOptions(
        baseUrl: _config.apiBaseUrl,
        connectTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
        headers: {'Content-Type': 'multipart/form-data'},
      ),
    );

    // Add auth token
    final token = await _tokenStorage.readAccessToken();
    if (token != null && token.isNotEmpty) {
      dio.options.headers['Authorization'] = 'Bearer $token';
    }

    try {
      // Try contribution image upload endpoint first
      // If not available, we'll return null (image will be skipped)
      final response = await dio.post<Map<String, dynamic>>(
        '/api/contributions/upload-image',
        data: formData,
      );

      final data = response.data?['data'] as Map<String, dynamic>?;
      if (data == null) {
        // If response doesn't have expected format, return null
        return '';
      }

      // Return image URL
      return data['imageUrl'] as String? ??
          data['url'] as String? ??
          '';
    } on DioException catch (e) {
      // If endpoint doesn't exist (404) or other error, return empty string
      // Contribution will be submitted without image URL
      // In production, this endpoint should be implemented
      if (e.response?.statusCode == 404) {
        return ''; // Endpoint not implemented yet
      }
      rethrow;
    } catch (e) {
      // For other errors, return empty string
      return '';
    } finally {
      dio.close();
    }
  }
}

final imageUploadRepositoryProvider = Provider<ImageUploadRepository>((ref) {
  final config = ref.watch(appConfigProvider);
  final tokenStorage = ref.watch(tokenStorageProvider);
  return ImageUploadRepository(config, tokenStorage);
});

