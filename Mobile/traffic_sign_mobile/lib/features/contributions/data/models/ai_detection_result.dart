class AIDetectionResult {
  AIDetectionResult({
    required this.detectedSigns,
    this.confidence,
  });

  factory AIDetectionResult.fromJson(Map<String, dynamic> json) {
    final detectedSigns = (json['detectedSigns'] as List<dynamic>?)
            ?.map((item) => DetectedSign.fromJson(item as Map<String, dynamic>))
            .toList() ??
        [];
    return AIDetectionResult(
      detectedSigns: detectedSigns,
      confidence: (json['confidence'] as num?)?.toDouble(),
    );
  }

  final List<DetectedSign> detectedSigns;
  final double? confidence;
}

class DetectedSign {
  DetectedSign({
    required this.type,
    required this.confidence,
    this.boundingBox,
  });

  factory DetectedSign.fromJson(Map<String, dynamic> json) {
    return DetectedSign(
      type: json['type'] as String? ?? '',
      confidence: (json['confidence'] as num).toDouble(),
      boundingBox: json['boundingBox'] != null
          ? BoundingBox.fromJson(json['boundingBox'] as Map<String, dynamic>)
          : null,
    );
  }

  final String type;
  final double confidence;
  final BoundingBox? boundingBox;
}

class BoundingBox {
  BoundingBox({
    required this.x,
    required this.y,
    required this.width,
    required this.height,
  });

  factory BoundingBox.fromJson(Map<String, dynamic> json) {
    return BoundingBox(
      x: (json['x'] as num).toDouble(),
      y: (json['y'] as num).toDouble(),
      width: (json['width'] as num).toDouble(),
      height: (json['height'] as num).toDouble(),
    );
  }

  final double x;
  final double y;
  final double width;
  final double height;
}

