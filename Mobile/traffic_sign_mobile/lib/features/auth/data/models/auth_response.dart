import 'auth_user.dart';

class AuthResponse {
  AuthResponse({required this.user, this.token, this.message});

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      user: AuthUser.fromJson(json['user'] as Map<String, dynamic>),
      token: json['token'] as String?,
      message: json['message'] as String?,
    );
  }

  final AuthUser user;
  final String? token;
  final String? message;
}
