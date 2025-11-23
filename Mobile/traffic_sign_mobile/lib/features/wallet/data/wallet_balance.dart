class WalletBalance {
  WalletBalance({
    required this.userId,
    required this.username,
    required this.balance,
  });

  factory WalletBalance.fromJson(Map<String, dynamic> json) {
    // Support both camelCase and PascalCase for compatibility
    final userIdValue = json['userId'] ?? json['UserId'];
    final usernameValue = json['username'] ?? json['Username'];
    final balanceValue = json['balance'] ?? json['Balance'];
    
    return WalletBalance(
      userId: userIdValue as int,
      username: usernameValue as String? ?? '',
      balance: (balanceValue as num).toDouble(),
    );
  }

  final int userId;
  final String username;
  final double balance;
}
