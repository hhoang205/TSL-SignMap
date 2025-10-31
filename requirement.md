Bạn là kỹ sư lập trình chuyên môn cao. Đây là yêu đề tài tôi làm trong kì học này hãy hướng dẫn tôi từng bước để có thế hoàn thành nó. Nếu được hay chia task nhỏ và bắt buộc phải có microservice.
English: AI-Enabled Community-based Traffic Sign Location Management System in VietNam
Vietnamese: Hệ thống quản lý vị trí biển báo giao thông dựa trên cộng đồng, tích hợp AI tại Việt
Nam
Abbreviation: SignMap
a. Context:
In VietNam, traffic signs are critical for ensuring road safety, regulating traffic flow, and
providing drivers with essential information for informed decision-making. Frequent
changes in traffic infrastructure—due to road construction, urban development, or
maintenance—require constant updates to traffic sign placements, including additions,
removals, relocations, or replacements. Traditional management approaches, which rely
on manual surveys and inspections, are time-intensive, costly, and susceptible to human
error, resulting in outdated or incomplete data. This increases the risk of traffic violations
and accidents.
To overcome these challenges, the AI-powered Traffic Sign Location Management System
(TSL) is proposed. TSL leverages mobile technology, community contributions, and
artificial intelligence to maintain a real-time, collaborative traffic sign database. The
system features a user-friendly mobile application integrated with OpenStreetMap or

x

11.14-BM/DH/HDCV/FU 1/0
similar open-source mapping services, displaying up-to-date traffic sign locations to enable
safer and more confident navigation for drivers. Key features include:
● Crowdsourced Data Collection: Drivers and local residents can report, update, and
verify traffic sign information through the mobile app, ensuring timely and accurate
data.
● AI-Powered Computer Vision: Advanced models, such as YOLO, automatically
detect and classify traffic signs from user-uploaded images, enhancing data
accuracy and minimizing manual effort.
● Collaborative Verification: A voting and reputation system ensures the reliability
of user submissions, maintaining data integrity.
● Incentive Mechanism: A coin-based reward system encourages active community
participation.
● Real-Time Synchronization: Traffic sign updates are seamlessly integrated into the
map, providing users and authorities with current information.
By combining AI automation, community contributions, and real-time mapping, TSL
offers a scalable, efficient, and cost-effective solution for traffic sign management. The
system enhances road safety, improves compliance with traffic regulations, and supports
better navigation by delivering accurate, up-to-date traffic sign data to both users and
authorities.
b. Proposed Solution:
Develop a system named TSL, used in Vietnam, that leverages mobile technology,
community contributions, and artificial intelligence to maintain a real-time, collaborative
traffic sign database. The system includes:
- Mobile App with Real-Time Maps: A user-friendly mobile app integrates with
OpenStreetMap or similar open-license services to display real-time traffic sign locations,
aiding drivers in safer navigation.
- User Contributions: Users can submit updates—such as missing signs, incorrect
placements, or new additions—to the traffic sign database.
- Verification Process: A robust system ensures data integrity by having administrators
review and approve user submissions for accuracy and relevance before database
integration.
- Voting Mechanism: Registered users with minimal activity vote (upvote/downvote) on
submissions, with votes weighted by reputation, proximity, and expertise. Submissions
scoring over 70% (after 5+ votes or 7 days) are accepted, below 30% rejected, and 30%-
70% flagged for admin review. Admins can override outcomes, notifying users and
awarding reputation points to boost participation.
- Collaborative Platform: The system encourages collaboration between users and admins to

improve the traffic sign database’s accuracy and completeness, fostering community-
driven road safety.

- TSL Contribution Cycle: Users earn TSL Coins for approved submissions (e.g., 10+ Coins)
and aligned votes (1 Coin), spending them on app features like map access (2 Coins/day)
or submissions (5 Coins). Users can top up coins with money (e.g., $1 for 10 Coins), with
balances tracked and admins adjusting rewards/penalties to sustain engagement and the
app’s economy.

11.14-BM/DH/HDCV/FU 1/0
- Notifications and Updates: Users receive real-time notifications about approved changes,
ensuring access to current traffic sign data for regulatory compliance.
- User-Friendly Interface: The app offers an intuitive design for easy searching, navigation,
and contributions to the traffic sign database.
c. Functional Requirement:
User Registration and Authentication:
● Users must register accounts within the mobile app, receiving 20 initial TSL Coins.
● The system authenticates and authorizes users for secure feature access.
Traffic Sign Display:
● The app integrates with OpenStreetMap or similar services to show real-time traffic
sign locations.
● Displays various sign types (e.g., regulatory, warning, informational) on the map.
Traffic Sign Search and Filtering:
● Users can search signs by type or location, costing 1 Coin for advanced filters.
● Filtering options include sign type or proximity (e.g., within a user-defined radius).
User Contributions:
● Users submit updates (new, missing, or incorrect signs) with details (type,
location), costing 5 Coins each.
● Submissions allow optional descriptions or photos for verification.
Voting Mechanism:
● Eligible users can vote on submissions (upvote/downvote).
● Earn 1 Coin per aligned vote (max 5/day).
● Votes are weighted by reputation, proximity, and expertise.
Admin Verification:
● Admins access a web panel to review submissions, approve/reject them, and
override votes if needed.
● Users are notified of submission status (approved, rejected, or pending).
Data Integration and Import:
● Approved updates are securely integrated into the database, triggering coin rewards
(e.g., 10+ Coins).
● The system ensures efficient data handling and updates.
Notifications and Updates:
● Users receive real-time alerts on approved changes and submission outcomes.
● Updates reflect the latest traffic sign data for compliance.
User Profile Management:
● Users manage profiles (personal info, settings, coin balance) and can edit/delete
submissions.
● Coin top-up option allows purchases (e.g., $1 for 10 Coins) via in-app payment.
User Feedback and Reporting:
● Users can submit feedback or report issues with the app or database.
● Reporting inappropriate content or misuse is supported.
System Administration:
● Admins manage user accounts, permissions, and coin adjustments via the web
panel.
● Analytics on user activity and sign data are available.
System Security:

11.14-BM/DH/HDCV/FU 1/0

● Prevents unauthorized access
Compatibility and Scalability:
● The mobile app supports iOS or Android platforms for normal users
● The web app supports for Admins/Staffs
(*) 3.2. Main proposal content (including result and product)
a) Theory and practice (document):
• Theoretical Foundations:
● Crowdsourced data validation using weighted voting based on user reputation,
proximity, and expertise.
● Assignment algorithms (e.g., Hungarian algorithm) to detect modifications,
insertions, or deletions in the traffic sign dataset.
● TSL Contribution Cycle integrating coin-based incentives for user engagement and
data accuracy.
• Deliverables:
● Comprehensive documentation, including User Requirements, Software
Requirement Specification, Architecture Design, Detailed Design, System
Implementation, Testing Plan, Installation Guide, source code, and deployable
software packages.
• Server-Side Technologies:
● Framework: .NET, Java, or similar, hosted on cloud platforms(e.g., Windows
Azure)
● Database: MS SQL Server, MySQL, or any suitable DBMS for storing traffic sign
and user data.
• Client-Side Technologies:
● Web Client: ReactJS, NextJS, Angular, or any suitable technologies for the admin
and staff interfaces.