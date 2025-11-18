import 'package:equatable/equatable.dart';

class AuthUser extends Equatable {
  const AuthUser({
    required this.id,
    required this.username,
    required this.email,
    required this.role,
    required this.reputation,
    this.phoneNumber,
  });

  factory AuthUser.fromJson(Map<String, dynamic> json) {
    return AuthUser(
      id: json['id'] as int,
      username: json['username'] as String? ?? '',
      email: json['email'] as String? ?? '',
      role: json['role'] as String? ?? 'User',
      reputation: (json['reputation'] as num?)?.toDouble() ?? 0,
      phoneNumber: json['phoneNumber'] as String?,
    );
  }

  final int id;
  final String username;
  final String email;
  final String role;
  final double reputation;
  final String? phoneNumber;

  Map<String, dynamic> toJson() => {
    'id': id,
    'username': username,
    'email': email,
    'role': role,
    'reputation': reputation,
    'phoneNumber': phoneNumber,
  };

  @override
  List<Object?> get props => [
    id,
    username,
    email,
    role,
    reputation,
    phoneNumber,
  ];
}
