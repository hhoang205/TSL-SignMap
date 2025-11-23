import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_vi.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('vi'),
  ];

  /// The application title
  ///
  /// In en, this message translates to:
  /// **'Traffic Sign'**
  String get appTitle;

  /// Welcome message on login screen
  ///
  /// In en, this message translates to:
  /// **'Welcome back!'**
  String get welcomeBack;

  /// Subtitle on login screen
  ///
  /// In en, this message translates to:
  /// **'Login to continue contributing traffic signs'**
  String get loginToContinue;

  /// Email field label
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// Email field hint
  ///
  /// In en, this message translates to:
  /// **'Enter your email'**
  String get enterYourEmail;

  /// Email validation error
  ///
  /// In en, this message translates to:
  /// **'Please enter email'**
  String get pleaseEnterEmail;

  /// Email format validation error
  ///
  /// In en, this message translates to:
  /// **'Invalid email'**
  String get invalidEmail;

  /// Password field label
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// Password field hint
  ///
  /// In en, this message translates to:
  /// **'Enter your password'**
  String get enterYourPassword;

  /// Password validation error
  ///
  /// In en, this message translates to:
  /// **'Please enter password'**
  String get pleaseEnterPassword;

  /// Password length validation error
  ///
  /// In en, this message translates to:
  /// **'Password must be at least 6 characters'**
  String get passwordMinLength;

  /// Login button text
  ///
  /// In en, this message translates to:
  /// **'Login'**
  String get login;

  /// Text before register link
  ///
  /// In en, this message translates to:
  /// **'Don\'t have an account? '**
  String get dontHaveAccount;

  /// Register link text
  ///
  /// In en, this message translates to:
  /// **'Register now'**
  String get registerNow;

  /// Registration success message
  ///
  /// In en, this message translates to:
  /// **'Registration successful, please login.'**
  String get registerSuccess;

  /// Register screen title
  ///
  /// In en, this message translates to:
  /// **'Create Account'**
  String get createAccount;

  /// Map tab label
  ///
  /// In en, this message translates to:
  /// **'Map'**
  String get map;

  /// Contributions tab label
  ///
  /// In en, this message translates to:
  /// **'Contributions'**
  String get contributions;

  /// Voting tab label
  ///
  /// In en, this message translates to:
  /// **'Voting'**
  String get voting;

  /// Wallet tab label
  ///
  /// In en, this message translates to:
  /// **'Wallet'**
  String get wallet;

  /// Notifications tab label
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notifications;

  /// Profile menu item
  ///
  /// In en, this message translates to:
  /// **'Profile'**
  String get profile;

  /// Account tooltip
  ///
  /// In en, this message translates to:
  /// **'Account'**
  String get account;

  /// Advanced search tooltip
  ///
  /// In en, this message translates to:
  /// **'Advanced search'**
  String get advancedSearch;

  /// Create contribution tooltip
  ///
  /// In en, this message translates to:
  /// **'Create new contribution'**
  String get createNewContribution;

  /// Refresh button tooltip
  ///
  /// In en, this message translates to:
  /// **'Refresh'**
  String get refresh;

  /// Close button text
  ///
  /// In en, this message translates to:
  /// **'Close'**
  String get close;

  /// Location info dialog title
  ///
  /// In en, this message translates to:
  /// **'Location Information'**
  String get locationInfo;

  /// Coordinates display
  ///
  /// In en, this message translates to:
  /// **'Coordinates: {lat}, {lng}'**
  String coordinates(String lat, String lng);

  /// Status label
  ///
  /// In en, this message translates to:
  /// **'Status'**
  String get status;

  /// Valid from label
  ///
  /// In en, this message translates to:
  /// **'Valid from'**
  String get validFrom;

  /// Valid to label
  ///
  /// In en, this message translates to:
  /// **'Valid to'**
  String get validTo;

  /// Error message when data loading fails
  ///
  /// In en, this message translates to:
  /// **'Cannot load data: {error}'**
  String cannotLoadData(String error);

  /// Try again button text
  ///
  /// In en, this message translates to:
  /// **'Try again'**
  String get tryAgain;

  /// Message when no data is available
  ///
  /// In en, this message translates to:
  /// **'No data available.'**
  String get noData;

  /// Message when user has no contributions
  ///
  /// In en, this message translates to:
  /// **'You don\'t have any contributions yet.'**
  String get noContributions;

  /// New contribution button text
  ///
  /// In en, this message translates to:
  /// **'New Contribution'**
  String get newContribution;

  /// Message when there are no notifications
  ///
  /// In en, this message translates to:
  /// **'No notifications yet.'**
  String get noNotifications;

  /// Error message when notifications loading fails
  ///
  /// In en, this message translates to:
  /// **'Cannot load notifications: {error}'**
  String cannotLoadNotifications(String error);

  /// Message when there are no pending contributions
  ///
  /// In en, this message translates to:
  /// **'No pending contributions to vote on.'**
  String get noPendingContributions;

  /// Sign type label
  ///
  /// In en, this message translates to:
  /// **'Sign Type: {type}'**
  String signType(String type);

  /// Agree button text
  ///
  /// In en, this message translates to:
  /// **'Agree'**
  String get agree;

  /// Disagree button text
  ///
  /// In en, this message translates to:
  /// **'Disagree'**
  String get disagree;

  /// Message when wallet data is not available
  ///
  /// In en, this message translates to:
  /// **'No wallet data available.'**
  String get noWalletData;

  /// Add coins button text
  ///
  /// In en, this message translates to:
  /// **'Add more coins'**
  String get addMoreCoins;

  /// Error message when wallet loading fails
  ///
  /// In en, this message translates to:
  /// **'Cannot load wallet: {error}'**
  String cannotLoadWallet(String error);

  /// Update success message
  ///
  /// In en, this message translates to:
  /// **'Update successful'**
  String get updateSuccess;

  /// Change password screen title
  ///
  /// In en, this message translates to:
  /// **'Change Password'**
  String get changePassword;

  /// Cancel button text
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancel;

  /// New contribution form title
  ///
  /// In en, this message translates to:
  /// **'New Contribution'**
  String get newContributionTitle;

  /// Action type field label
  ///
  /// In en, this message translates to:
  /// **'Action Type'**
  String get actionType;

  /// Add action option
  ///
  /// In en, this message translates to:
  /// **'Add'**
  String get add;

  /// Update action option
  ///
  /// In en, this message translates to:
  /// **'Update'**
  String get update;

  /// Delete action option
  ///
  /// In en, this message translates to:
  /// **'Delete'**
  String get delete;

  /// Sign type field label
  ///
  /// In en, this message translates to:
  /// **'Sign Type'**
  String get signTypeLabel;

  /// Sign type validation error
  ///
  /// In en, this message translates to:
  /// **'Please enter sign type'**
  String get pleaseEnterSignType;

  /// Description field label
  ///
  /// In en, this message translates to:
  /// **'Detailed Description'**
  String get detailedDescription;

  /// Latitude display
  ///
  /// In en, this message translates to:
  /// **'Latitude: {lat}'**
  String latitude(String lat);

  /// Longitude display
  ///
  /// In en, this message translates to:
  /// **'Longitude: {lng}'**
  String longitude(String lng);

  /// Take photo button text
  ///
  /// In en, this message translates to:
  /// **'Take Photo'**
  String get takePhoto;

  /// Submit contribution button text
  ///
  /// In en, this message translates to:
  /// **'Submit Contribution (costs 5 coins)'**
  String get submitContribution;

  /// Dash character for empty values
  ///
  /// In en, this message translates to:
  /// **'-'**
  String get dash;

  /// Register screen header
  ///
  /// In en, this message translates to:
  /// **'Join the community'**
  String get joinCommunity;

  /// Register screen subtitle
  ///
  /// In en, this message translates to:
  /// **'Create an account to start contributing traffic signs'**
  String get createAccountToContribute;

  /// Username field label
  ///
  /// In en, this message translates to:
  /// **'Username'**
  String get username;

  /// Username field hint
  ///
  /// In en, this message translates to:
  /// **'Enter username'**
  String get enterUsername;

  /// Username validation error
  ///
  /// In en, this message translates to:
  /// **'Please enter username'**
  String get pleaseEnterUsername;

  /// Username length validation error
  ///
  /// In en, this message translates to:
  /// **'Username must be at least 3 characters'**
  String get usernameMinLength;

  /// Phone number field label
  ///
  /// In en, this message translates to:
  /// **'Phone Number (optional)'**
  String get phoneNumber;

  /// Phone number field hint
  ///
  /// In en, this message translates to:
  /// **'Enter phone number'**
  String get enterPhoneNumber;

  /// Password field hint
  ///
  /// In en, this message translates to:
  /// **'Minimum 6 characters'**
  String get passwordMinHint;

  /// Register button text
  ///
  /// In en, this message translates to:
  /// **'Register'**
  String get register;

  /// Text before login link
  ///
  /// In en, this message translates to:
  /// **'Already have an account? '**
  String get alreadyHaveAccount;

  /// Error message when location is required but not captured
  ///
  /// In en, this message translates to:
  /// **'Please capture your location first'**
  String get pleaseCaptureLocation;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'vi'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'vi':
      return AppLocalizationsVi();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
