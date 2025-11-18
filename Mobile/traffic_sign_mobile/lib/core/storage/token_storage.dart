import 'dart:convert';

import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../features/auth/data/models/auth_user.dart';

class TokenStorage {
  TokenStorage(this._secureStorage, this._preferences);

  final FlutterSecureStorage _secureStorage;
  final SharedPreferences _preferences;

  static const _accessTokenKey = 'ts_access_token';
  static const _userKey = 'ts_user';

  Future<void> saveSession({
    required String? token,
    required AuthUser user,
  }) async {
    if (token != null) {
      await _secureStorage.write(key: _accessTokenKey, value: token);
    }
    await _preferences.setString(_userKey, jsonEncode(user.toJson()));
  }

  Future<void> clear() async {
    await _secureStorage.delete(key: _accessTokenKey);
    await _preferences.remove(_userKey);
  }

  Future<String?> readAccessToken() =>
      _secureStorage.read(key: _accessTokenKey);

  AuthUser? getStoredUser() {
    final raw = _preferences.getString(_userKey);
    if (raw == null) return null;
    try {
      return AuthUser.fromJson(jsonDecode(raw) as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }
}

final tokenStorageProvider = Provider<TokenStorage>((ref) {
  throw UnimplementedError('TokenStorage must be initialized before use.');
});

Future<TokenStorage> initTokenStorage() async {
  const secureStorage = FlutterSecureStorage();
  final prefs = await SharedPreferences.getInstance();
  return TokenStorage(secureStorage, prefs);
}
