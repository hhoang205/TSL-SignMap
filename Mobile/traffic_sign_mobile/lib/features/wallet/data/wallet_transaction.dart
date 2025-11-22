import 'package:intl/intl.dart';

enum TransactionType {
  credit,
  debit,
  adjustment,
  payment,
  contribution,
  voting,
  unknown,
}

enum TransactionStatus {
  pending,
  completed,
  failed,
  cancelled,
}

class WalletTransaction {
  WalletTransaction({
    required this.id,
    required this.userId,
    required this.amount,
    required this.type,
    required this.status,
    required this.description,
    required this.createdAt,
    this.relatedId,
    this.relatedType,
  });

  factory WalletTransaction.fromJson(Map<String, dynamic> json) {
    return WalletTransaction(
      id: json['id'] as String? ?? json['id'].toString(),
      userId: json['userId'] as int,
      amount: (json['amount'] as num).toDouble(),
      type: _parseTransactionType(json['type'] as String?),
      status: _parseTransactionStatus(json['status'] as String?),
      description: json['description'] as String? ?? '',
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'] as String)
          : DateTime.now(),
      relatedId: json['relatedId'] as String?,
      relatedType: json['relatedType'] as String?,
    );
  }

  final String id;
  final int userId;
  final double amount;
  final TransactionType type;
  final TransactionStatus status;
  final String description;
  final DateTime createdAt;
  final String? relatedId;
  final String? relatedType;

  bool get isCredit => amount > 0;
  bool get isDebit => amount < 0;

  String get formattedAmount {
    final absAmount = amount.abs();
    return '${isCredit ? '+' : '-'}${absAmount.toStringAsFixed(1)} coins';
  }

  String get formattedDate {
    final formatter = DateFormat('dd/MM/yyyy HH:mm');
    return formatter.format(createdAt);
  }

  static TransactionType _parseTransactionType(String? type) {
    switch (type?.toLowerCase()) {
      case 'credit':
        return TransactionType.credit;
      case 'debit':
        return TransactionType.debit;
      case 'adjustment':
        return TransactionType.adjustment;
      case 'payment':
        return TransactionType.payment;
      case 'contribution':
        return TransactionType.contribution;
      case 'voting':
        return TransactionType.voting;
      default:
        return TransactionType.unknown;
    }
  }

  static TransactionStatus _parseTransactionStatus(String? status) {
    switch (status?.toLowerCase()) {
      case 'pending':
        return TransactionStatus.pending;
      case 'completed':
        return TransactionStatus.completed;
      case 'failed':
        return TransactionStatus.failed;
      case 'cancelled':
        return TransactionStatus.cancelled;
      default:
        return TransactionStatus.completed;
    }
  }
}

