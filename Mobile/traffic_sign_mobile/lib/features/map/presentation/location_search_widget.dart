import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../application/geocoding_controller.dart';
import '../data/models/geocoding_result.dart';

class LocationSearchWidget extends ConsumerStatefulWidget {
  const LocationSearchWidget({
    super.key,
    required this.onLocationSelected,
  });

  final void Function(GeocodingResult) onLocationSelected;

  @override
  ConsumerState<LocationSearchWidget> createState() =>
      _LocationSearchWidgetState();
}

class _LocationSearchWidgetState extends ConsumerState<LocationSearchWidget> {
  final _searchController = TextEditingController();
  Timer? _debounceTimer;

  @override
  void dispose() {
    _debounceTimer?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  void _onSearchChanged(String query) {
    setState(() {}); // Cập nhật UI cho suffixIcon
    _debounceTimer?.cancel();
    _debounceTimer = Timer(const Duration(milliseconds: 500), () {
      if (query.trim().isNotEmpty) {
        ref.read(geocodingControllerProvider.notifier).searchPlaces(query);
      } else {
        ref.read(geocodingControllerProvider.notifier).clearResults();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final geocodingState = ref.watch(geocodingControllerProvider);

    return Column(
      children: [
        TextField(
          controller: _searchController,
          decoration: InputDecoration(
            hintText: 'Tìm kiếm địa điểm...',
            prefixIcon: const Icon(Icons.search),
            suffixIcon: _searchController.text.isNotEmpty
                ? IconButton(
                    icon: const Icon(Icons.clear),
                    onPressed: () {
                      _searchController.clear();
                      ref.read(geocodingControllerProvider.notifier).clearResults();
                    },
                  )
                : null,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
            ),
            filled: true,
            fillColor: Colors.white,
          ),
          onChanged: _onSearchChanged,
        ),
        if (geocodingState.isLoading)
          const Padding(
            padding: EdgeInsets.all(8.0),
            child: CircularProgressIndicator(),
          ),
        if (geocodingState.error != null)
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Text(
              geocodingState.error!,
              style: TextStyle(color: Colors.red[700]),
            ),
          ),
        if (geocodingState.searchResults.isNotEmpty)
          Container(
            constraints: const BoxConstraints(maxHeight: 300),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.1),
                  blurRadius: 10,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: ListView.builder(
              shrinkWrap: true,
              itemCount: geocodingState.searchResults.length,
              itemBuilder: (context, index) {
                final result = geocodingState.searchResults[index];
                return ListTile(
                  leading: const Icon(Icons.location_on, color: Colors.red),
                  title: Text(
                    result.displayName,
                    style: const TextStyle(fontWeight: FontWeight.w500),
                  ),
                  subtitle: result.address != null
                      ? Text(result.address!.formattedAddress)
                      : null,
                  onTap: () {
                    widget.onLocationSelected(result);
                    _searchController.clear();
                    ref.read(geocodingControllerProvider.notifier).clearResults();
                  },
                );
              },
            ),
          ),
      ],
    );
  }
}

