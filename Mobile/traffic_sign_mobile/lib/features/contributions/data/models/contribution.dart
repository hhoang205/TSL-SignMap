class Contribution {
  Contribution({
    required this.id,
    required this.userId,
    required this.action,
    required this.description,
    required this.status,
    required this.createdAt,
    this.type,
    this.latitude,
    this.longitude,
    this.imageUrl,
  });

  factory Contribution.fromJson(Map<String, dynamic> json) {
    return Contribution(
      id: json['id'] as int,
      userId: json['userId'] as int,
      action: json['action'] as String? ?? 'Add',
      description: json['description'] as String? ?? '',
      status: json['status'] as String? ?? 'Pending',
      createdAt: DateTime.parse(json['createdAt'] as String),
      type: json['type'] as String?,
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
      imageUrl: json['imageUrl'] as String?,
    );
  }

  final int id;
  final int userId;
  final String action;
  final String description;
  final String status;
  final DateTime createdAt;
  final String? type;
  final double? latitude;
  final double? longitude;
  final String? imageUrl;
}
