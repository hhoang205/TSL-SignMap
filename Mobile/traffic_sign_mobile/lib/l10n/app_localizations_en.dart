// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'Traffic Sign';

  @override
  String get welcomeBack => 'Welcome back!';

  @override
  String get loginToContinue => 'Login to continue contributing traffic signs';

  @override
  String get email => 'Email';

  @override
  String get enterYourEmail => 'Enter your email';

  @override
  String get pleaseEnterEmail => 'Please enter email';

  @override
  String get invalidEmail => 'Invalid email';

  @override
  String get password => 'Password';

  @override
  String get enterYourPassword => 'Enter your password';

  @override
  String get pleaseEnterPassword => 'Please enter password';

  @override
  String get passwordMinLength => 'Password must be at least 6 characters';

  @override
  String get login => 'Login';

  @override
  String get dontHaveAccount => 'Don\'t have an account? ';

  @override
  String get registerNow => 'Register now';

  @override
  String get registerSuccess => 'Registration successful, please login.';

  @override
  String get createAccount => 'Create Account';

  @override
  String get map => 'Map';

  @override
  String get contributions => 'Contributions';

  @override
  String get voting => 'Voting';

  @override
  String get wallet => 'Wallet';

  @override
  String get notifications => 'Notifications';

  @override
  String get profile => 'Profile';

  @override
  String get account => 'Account';

  @override
  String get advancedSearch => 'Advanced search';

  @override
  String get createNewContribution => 'Create new contribution';

  @override
  String get refresh => 'Refresh';

  @override
  String get close => 'Close';

  @override
  String get locationInfo => 'Location Information';

  @override
  String coordinates(String lat, String lng) {
    return 'Coordinates: $lat, $lng';
  }

  @override
  String get status => 'Status';

  @override
  String get validFrom => 'Valid from';

  @override
  String get validTo => 'Valid to';

  @override
  String cannotLoadData(String error) {
    return 'Cannot load data: $error';
  }

  @override
  String get tryAgain => 'Try again';

  @override
  String get noData => 'No data available.';

  @override
  String get noContributions => 'You don\'t have any contributions yet.';

  @override
  String get newContribution => 'New Contribution';

  @override
  String get noNotifications => 'No notifications yet.';

  @override
  String cannotLoadNotifications(String error) {
    return 'Cannot load notifications: $error';
  }

  @override
  String get noPendingContributions => 'No pending contributions to vote on.';

  @override
  String signType(String type) {
    return 'Sign Type: $type';
  }

  @override
  String get agree => 'Agree';

  @override
  String get disagree => 'Disagree';

  @override
  String get noWalletData => 'No wallet data available.';

  @override
  String get addMoreCoins => 'Add more coins';

  @override
  String cannotLoadWallet(String error) {
    return 'Cannot load wallet: $error';
  }

  @override
  String get updateSuccess => 'Update successful';

  @override
  String get changePassword => 'Change Password';

  @override
  String get cancel => 'Cancel';

  @override
  String get newContributionTitle => 'New Contribution';

  @override
  String get actionType => 'Action Type';

  @override
  String get add => 'Add';

  @override
  String get update => 'Update';

  @override
  String get delete => 'Delete';

  @override
  String get signTypeLabel => 'Sign Type';

  @override
  String get pleaseEnterSignType => 'Please enter sign type';

  @override
  String get detailedDescription => 'Detailed Description';

  @override
  String latitude(String lat) {
    return 'Latitude: $lat';
  }

  @override
  String longitude(String lng) {
    return 'Longitude: $lng';
  }

  @override
  String get takePhoto => 'Take Photo';

  @override
  String get submitContribution => 'Submit Contribution (costs 5 coins)';

  @override
  String get dash => '-';

  @override
  String get joinCommunity => 'Join the community';

  @override
  String get createAccountToContribute =>
      'Create an account to start contributing traffic signs';

  @override
  String get username => 'Username';

  @override
  String get enterUsername => 'Enter username';

  @override
  String get pleaseEnterUsername => 'Please enter username';

  @override
  String get usernameMinLength => 'Username must be at least 3 characters';

  @override
  String get phoneNumber => 'Phone Number (optional)';

  @override
  String get enterPhoneNumber => 'Enter phone number';

  @override
  String get passwordMinHint => 'Minimum 6 characters';

  @override
  String get register => 'Register';

  @override
  String get alreadyHaveAccount => 'Already have an account? ';

  @override
  String get pleaseCaptureLocation => 'Please capture your location first';
}
