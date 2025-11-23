// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Vietnamese (`vi`).
class AppLocalizationsVi extends AppLocalizations {
  AppLocalizationsVi([String locale = 'vi']) : super(locale);

  @override
  String get appTitle => 'Biển Báo Giao Thông';

  @override
  String get welcomeBack => 'Chào mừng trở lại!';

  @override
  String get loginToContinue =>
      'Đăng nhập để tiếp tục đóng góp biển báo giao thông';

  @override
  String get email => 'Email';

  @override
  String get enterYourEmail => 'Nhập email của bạn';

  @override
  String get pleaseEnterEmail => 'Vui lòng nhập email';

  @override
  String get invalidEmail => 'Email không hợp lệ';

  @override
  String get password => 'Mật khẩu';

  @override
  String get enterYourPassword => 'Nhập mật khẩu của bạn';

  @override
  String get pleaseEnterPassword => 'Vui lòng nhập mật khẩu';

  @override
  String get passwordMinLength => 'Mật khẩu phải có ít nhất 6 ký tự';

  @override
  String get login => 'Đăng nhập';

  @override
  String get dontHaveAccount => 'Chưa có tài khoản? ';

  @override
  String get registerNow => 'Đăng ký ngay';

  @override
  String get registerSuccess => 'Đăng ký thành công, vui lòng đăng nhập.';

  @override
  String get createAccount => 'Tạo tài khoản';

  @override
  String get map => 'Bản đồ';

  @override
  String get contributions => 'Đóng góp';

  @override
  String get voting => 'Bỏ phiếu';

  @override
  String get wallet => 'Ví';

  @override
  String get notifications => 'Thông báo';

  @override
  String get profile => 'Hồ sơ';

  @override
  String get account => 'Tài khoản';

  @override
  String get advancedSearch => 'Tìm kiếm nâng cao';

  @override
  String get createNewContribution => 'Tạo đóng góp mới';

  @override
  String get refresh => 'Làm mới';

  @override
  String get close => 'Đóng';

  @override
  String get locationInfo => 'Thông tin vị trí';

  @override
  String coordinates(String lat, String lng) {
    return 'Tọa độ: $lat, $lng';
  }

  @override
  String get status => 'Trạng thái';

  @override
  String get validFrom => 'Có hiệu lực từ';

  @override
  String get validTo => 'Có hiệu lực đến';

  @override
  String cannotLoadData(String error) {
    return 'Không thể tải dữ liệu: $error';
  }

  @override
  String get tryAgain => 'Thử lại';

  @override
  String get noData => 'Không có dữ liệu.';

  @override
  String get noContributions => 'Bạn chưa có đóng góp nào.';

  @override
  String get newContribution => 'Tạo đóng góp mới';

  @override
  String get noNotifications => 'Chưa có thông báo nào.';

  @override
  String cannotLoadNotifications(String error) {
    return 'Không thể tải thông báo: $error';
  }

  @override
  String get noPendingContributions => 'Không có đóng góp nào chờ duyệt.';

  @override
  String signType(String type) {
    return 'Loại biển báo: $type';
  }

  @override
  String get agree => 'Đồng ý';

  @override
  String get disagree => 'Không đồng ý';

  @override
  String get noWalletData => 'Không có dữ liệu ví.';

  @override
  String get addMoreCoins => 'Nạp thêm coin';

  @override
  String cannotLoadWallet(String error) {
    return 'Không thể tải ví: $error';
  }

  @override
  String get updateSuccess => 'Cập nhật thông tin thành công';

  @override
  String get changePassword => 'Đổi mật khẩu';

  @override
  String get cancel => 'Hủy';

  @override
  String get newContributionTitle => 'Đóng góp mới';

  @override
  String get actionType => 'Loại hành động';

  @override
  String get add => 'Thêm mới';

  @override
  String get update => 'Cập nhật';

  @override
  String get delete => 'Xóa';

  @override
  String get signTypeLabel => 'Loại biển báo';

  @override
  String get pleaseEnterSignType => 'Vui lòng nhập loại biển báo';

  @override
  String get detailedDescription => 'Mô tả chi tiết';

  @override
  String latitude(String lat) {
    return 'Vĩ độ: $lat';
  }

  @override
  String longitude(String lng) {
    return 'Kinh độ: $lng';
  }

  @override
  String get takePhoto => 'Chụp ảnh biển báo';

  @override
  String get submitContribution => 'Gửi đóng góp (tốn 5 coin)';

  @override
  String get dash => '-';

  @override
  String get joinCommunity => 'Tham gia cộng đồng';

  @override
  String get createAccountToContribute =>
      'Tạo tài khoản để bắt đầu đóng góp biển báo giao thông';

  @override
  String get username => 'Tên người dùng';

  @override
  String get enterUsername => 'Nhập tên người dùng';

  @override
  String get pleaseEnterUsername => 'Vui lòng nhập tên người dùng';

  @override
  String get usernameMinLength => 'Tên người dùng phải có ít nhất 3 ký tự';

  @override
  String get phoneNumber => 'Số điện thoại (tùy chọn)';

  @override
  String get enterPhoneNumber => 'Nhập số điện thoại';

  @override
  String get passwordMinHint => 'Tối thiểu 6 ký tự';

  @override
  String get register => 'Đăng ký';

  @override
  String get alreadyHaveAccount => 'Đã có tài khoản? ';

  @override
  String get pleaseCaptureLocation => 'Vui lòng lấy vị trí trước';
}
