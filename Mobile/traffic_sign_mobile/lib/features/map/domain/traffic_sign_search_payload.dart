class TrafficSignSearchPayload {
  TrafficSignSearchPayload({
    this.type,
    this.latitude,
    this.longitude,
    this.radiusKm,
    this.userId,
  });

  final String? type;
  final double? latitude;
  final double? longitude;
  final double? radiusKm;
  final int? userId;

  Map<String, dynamic> toJson() => {
    if (type != null && type!.isNotEmpty) 'type': type,
    if (latitude != null) 'latitude': latitude,
    if (longitude != null) 'longitude': longitude,
    if (radiusKm != null) 'radiusKm': radiusKm,
    if (userId != null) 'userId': userId,
  };
}
