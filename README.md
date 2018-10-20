# Dynamic Crm 365 Instant Notification

Dynamic CRM 365 Instant Notification is a plugin that enables CRM to send notifications to users instantly for different entities. you can send notification with additional data to a specific user or any organization inside Dynamic CRM 365. 
<br/>
How to Setup:<br/>
1-Build Adin Pusher Project and host it on your iis server.<br/>
2-Change wsUrlBase and pusherUrlBase in AdinNotifier.js <br/>
3-Copy AdinNotifier.js to \_static\_common\scripts folder<br/>
4-Add AdinNotifier.js to the first line of head section in main.aspx file in dynamic crm folder  <br/> 
```
	<script  type="text/javascript" src="/_static/_common/scripts/AdinNotifier.js"/>
```	
5-Test Notification system by PusherTest<br/>
6-Add Notification to crm plugins like SendNotificationPlugin sample code<br/>
	

![Dynamic Crm 365 Push Notification](http://soghandi.com/img/notification.jpg)



