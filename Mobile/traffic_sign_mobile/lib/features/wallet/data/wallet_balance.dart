class WalletBalance {
  WalletBalance({
    required this.userId,
    required this.username,
    required this.balance,
  });

  factory WalletBalance.fromJson(Map<String, dynamic> json) {
    return WalletBalance(
      userId: json['userId'] as int,
      username: json['username'] as String? ?? '',
      balance: (json['balance'] as num).toDouble(),
    );
  }

  final int userId;
  final String username;
  final double balance;
}
