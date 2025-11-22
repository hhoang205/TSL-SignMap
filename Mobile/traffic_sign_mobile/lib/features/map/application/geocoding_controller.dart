import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/models/geocoding_result.dart';
import '../data/openstreetmap_service.dart';

final openStreetMapServiceProvider = Provider<OpenStreetMapService>((ref) {
  return OpenStreetMapService();
});

final geocodingControllerProvider =
    StateNotifierProvider<GeocodingController, GeocodingState>((ref) {
  return GeocodingController(ref.read(openStreetMapServiceProvider));
});

class GeocodingState {
  GeocodingState({
    this.searchResults = const [],
    this.selectedLocation,
    this.isLoading = false,
    this.error,
  });

  final List<GeocodingResult> searchResults;
  final GeocodingResult? selectedLocation;
  final bool isLoading;
  final String? error;

  GeocodingState copyWith({
    List<GeocodingResult>? searchResults,
    GeocodingResult? selectedLocation,
    bool? isLoading,
    String? error,
  }) {
    return GeocodingState(
      searchResults: searchResults ?? this.searchResults,
      selectedLocation: selectedLocation ?? this.selectedLocation,
      isLoading: isLoading ?? this.isLoading,
      error: error,
    );
  }
}

class GeocodingController extends StateNotifier<GeocodingState> {
  GeocodingController(this._service) : super(GeocodingState());

  final OpenStreetMapService _service;

  Future<void> searchPlaces(String query) async {
    if (query.trim().isEmpty) {
      state = state.copyWith(searchResults: [], isLoading: false);
      return;
    }

    state = state.copyWith(isLoading: true, error: null);
    try {
      final results = await _service.searchPlaces(query);
      state = state.copyWith(
        searchResults: results,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  Future<void> reverseGeocode(double latitude, double longitude) async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      final result = await _service.reverseGeocode(latitude, longitude);
      state = state.copyWith(
        selectedLocation: result,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  Future<void> geocode(String address) async {
    state = state.copyWith(isLoading: true, error: null);
    try {
      final results = await _service.geocode(address);
      state = state.copyWith(
        searchResults: results,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  void selectLocation(GeocodingResult location) {
    state = state.copyWith(selectedLocation: location);
  }

  void clearResults() {
    state = state.copyWith(searchResults: [], selectedLocation: null);
  }
}

