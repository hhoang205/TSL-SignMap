class TrafficSign {
  TrafficSign({
    required this.id,
    required this.type,
    required this.latitude,
    required this.longitude,
    required this.status,
    required this.validFrom,
    this.validTo,
    this.imageUrl,
  });

  factory TrafficSign.fromJson(Map<String, dynamic> json) {
    return TrafficSign(
      id: json['id'] as int,
      type: json['type'] as String? ?? 'Unknown',
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
      status: json['status'] as String? ?? 'Active',
      validFrom: DateTime.parse(json['validFrom'] as String),
      validTo: json['validTo'] != null
          ? DateTime.tryParse(json['validTo'] as String)
          : null,
      imageUrl: json['imageUrl'] as String?,
    );
  }

  final int id;
  final String type;
  final double latitude;
  final double longitude;
  final String status;
  final DateTime validFrom;
  final DateTime? validTo;
  final String? imageUrl;
}
