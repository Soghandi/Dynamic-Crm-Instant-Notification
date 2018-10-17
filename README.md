# Dynamic Crm 365 Instant Notification

Dynamic Crm 365 Instant Notification is a plugin that enables Microsoft Dynamic CRM 365 to send notifications to users instantly for different entities. you can add all the send notification with additional data to specific user or any organization in dynamic crm 365.

How to Setup:

Step 1:
	1-Build Adin Pusher Project and host it on your iis server.
	2-Change wsUrlBase and pusherUrlBase in AdinNotifier.js 
	3-Copy AdinNotifier.js to \_static\_common\scripts folder
	4-Add AdinNotifier.js to the first line of head section in main.aspx file in dynamic crm folder ==>  <script  type="text/javascript" src="/_static/_common/scripts/AdinNotifier.js"/>
	5-Test Notification system by PusherTest
	6-Add Notification to crm plugins like SendNotificationPlugin sample code
	




