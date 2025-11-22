class GeocodingResult {
  GeocodingResult({
    required this.placeId,
    required this.displayName,
    required this.latitude,
    required this.longitude,
    this.address,
    this.type,
    this.importance,
  });

  factory GeocodingResult.fromJson(Map<String, dynamic> json) {
    return GeocodingResult(
      placeId: json['place_id'] as int,
      displayName: json['display_name'] as String? ?? '',
      latitude: double.parse(json['lat'] as String),
      longitude: double.parse(json['lon'] as String),
      address: json['address'] != null
          ? Address.fromJson(json['address'] as Map<String, dynamic>)
          : null,
      type: json['type'] as String?,
      importance: (json['importance'] as num?)?.toDouble(),
    );
  }

  final int placeId;
  final String displayName;
  final double latitude;
  final double longitude;
  final Address? address;
  final String? type;
  final double? importance;
}

class Address {
  Address({
    this.road,
    this.houseNumber,
    this.neighbourhood,
    this.suburb,
    this.city,
    this.state,
    this.country,
    this.postcode,
  });

  factory Address.fromJson(Map<String, dynamic> json) {
    return Address(
      road: json['road'] as String?,
      houseNumber: json['house_number'] as String?,
      neighbourhood: json['neighbourhood'] as String?,
      suburb: json['suburb'] as String?,
      city: json['city'] as String? ?? json['town'] as String?,
      state: json['state'] as String?,
      country: json['country'] as String?,
      postcode: json['postcode'] as String?,
    );
  }

  final String? road;
  final String? houseNumber;
  final String? neighbourhood;
  final String? suburb;
  final String? city;
  final String? state;
  final String? country;
  final String? postcode;

  String get formattedAddress {
    final parts = <String>[];
    if (houseNumber != null && road != null) {
      parts.add('$houseNumber $road');
    } else if (road != null) {
      parts.add(road!);
    }
    if (neighbourhood != null) parts.add(neighbourhood!);
    if (suburb != null) parts.add(suburb!);
    if (city != null) parts.add(city!);
    if (state != null) parts.add(state!);
    if (country != null) parts.add(country!);
    return parts.join(', ');
  }
}

