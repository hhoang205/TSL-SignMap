class VoteSummary {
  VoteSummary({
    required this.contributionId,
    required this.totalVotes,
    required this.upvotes,
    required this.downvotes,
    required this.averageWeight,
    required this.totalScore,
  });

  factory VoteSummary.fromJson(Map<String, dynamic> json) {
    return VoteSummary(
      contributionId: json['contributionId'] as int,
      totalVotes: json['totalVotes'] as int,
      upvotes: json['upvotes'] as int,
      downvotes: json['downvotes'] as int,
      averageWeight: (json['averageWeight'] as num?)?.toDouble() ?? 0,
      totalScore: (json['totalScore'] as num?)?.toDouble() ?? 0,
    );
  }

  final int contributionId;
  final int totalVotes;
  final int upvotes;
  final int downvotes;
  final double averageWeight;
  final double totalScore;
}
