---
page_type: sample
languages:
- csharp
products:
- office-teams
description: New employee on boarding app will simplify the new employee on boarding using MS Teams and SharePoint Online.
urlFragment: microsoft-teams-apps-newemployeeonboarding
---

# New Employee Onboarding App Template
| [Documentation](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/Home) | [Deployment guide](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/Deployment-Guide) | [Architecture](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/Solution-overview) |
| ---- | ---- | ---- |

New Employee Onboarding enables your organization to connect new employees to people & culture and provide them with consistent experience and information to be productive faster. 

Built with - [SharePoint New Employee Onboarding Solution](https://lookbook.microsoft.com/details/75e60a32-9849-4ed4-b83e-b2b08983ad19) with deep integration in Teams, NEO app makes it super easy for Human resources to manage relevant content and process for new employees using New Employeee Checklist. This list can guide the new hire through the onboarding journey. The checklist can be corporate or departmental. As an organization, you can choose to deploy complete SharePoint Site or just create a New Hire Checklist which can be integrated with Teams app.

Additionally, new hires can also introduce themselves using the app by sharing information which is automatically shared with their managers. This automation makes it easier for managers to review introductions about all new hires and share them with the relevant teams in one shot! 

At any time, new employees can share feedback on a task in their onboarding journey or on the overall experience using a helpful bot command. All feedback is shared with the HR team through helpful notifications and can be downloaded. Moreover, HR teams can also use the app to share pulse surveys for new employees and review the employee feedback using Microsoft Forms.


Key features of the NEO App

- Offers a **consistent and high-quality onboarding experience** across organization
- **Connects new employees to their colleagues** with an icebreaker
- Empowers stakeholders to easily contribute towards new employee onboarding
- Reminds New Hires of the weekly tasks according to the new hire checklist
- Enables Human Resources Teams collect feedback to assist new employees in onboarding 

User Personas 

- **New Hire (Employee)** - New employee who has spent less than X days in the Organization. HR teams can set the X (number of days an employee is considered a new hire, ex: 90 days) so that new employees stop getting notifications post this period
- **HR Team (Learning Coordinator)** - Admin team who has access to manage content for New Employee Checklist, share pulse surveys and review overall feedback
- **Hiring Manager** - Managers of New Employees who can post introductions and help new employees in their journey

Simplified Workflow of the NEO App

- HR/Admin organizes New Employee Checklist for new employees. This is a simple SharePoint list.
- New employees receive a welcome card as soon as the app pre-installs.
- Managers review quick introduction of new employees
- Managers can review and post all introductions in one go using the bot command 
- New employees can use the New Employee Checklist to find all resources/tasks.
- Human Resources/Admin teams can set the survey to encourage new employees to share their feedback.
- New employees receive weekly notification by the NEO app for tasks for the current week. 
- New employees are enabled to share feedback on a resource/task. 
- New employees to get to know more about their extended team with Icebreaker feature and catch-up with colleagues for a quick virtual coffee/connect
- New employees can check the complete New Employee Checklist anytime using the tab inside the app

**Personal Scope:** User can install the bot in personal scope. Behavior of the application will vary according to user roles whether user is an HR, Hiring Manager or a New Hire. 

**New Hire:** 

* New employees receive a welcome card as soon as IT team adds to the new employee group in Azure

![New employee welcome card](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/NewEmployee-WelcomeCard.png) 

* NEO app prompts the new employee to write a fun introduction. Questions can be set by the HR team at the time of deployment.

![New hire introduce screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/NewHireIntro.png) 

* New employees can view tasks for the current week by clicking on weekly plan or use the New Employee Checklist tab to view all items in checklist and resources

![New hire weekly plan screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/WeeklyLearningPlan.PNG) 

**Hiring Manager:** 

* App is automatically installed for the hiring manager of the New Employee

![HM welcome screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/HM_WelcomeCard.png) 

* Managers can review multiple new hire introductions

![Hiring manager review introduction screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/HM_ReviewIntro.png) 

* Managers can share feedback for new employees' introductions or post them on the selected channel to introduce new employees to the extended teams

![Hiring manager approve introduction screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/HM_Approve.png) 

![Hiring manager select channel screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/ApproveIntroCard.png) 

* New employee introductions immediately appear in the posted channel for team members to start interacting with the new employee 

![Hiring manager post intro screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/HM_PostIntroInTeam.png) 

**New employee check list tab:** 

* New employee checklist can be used to view all items in the checklist and resources. This is a SharePoint list and connects Teams app to SharePoint New Employee Onboarding Solution. This list can be easily managed by HR team. All updates will be automatically picked up the Teams NEO app

![new employee check list screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/NewEmployeeCheckListTab.png) 

**HR:** 

![HR welcome screen](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/HRTeamWelcomeCard.png) 

**Team Scope:** Bot can be installed in Teams scope. It will be installed by HR in a particular team where required. 

- **Feedback tab:** 

* HR team can download the feedback shared by new hires on the overall app and process

![Team Scope](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/images/Feedback.PNG) 
 


## Get Started
Begin with the [Solution overview](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/Solution-overview) section to read about what the app does and how it works. 

When you are ready to try out NEO App, or to use it in your own organization, follow the steps in the [Deployment guide](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/wiki/Deployment-Guide). 

## Legal notice

This app template is provided under the [MIT License](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/blob/master/LICENSE) terms.  In addition to these terms, by using this app template you agree to the following:

- You, not Microsoft, will license the use of your app to users or organization. 

- This app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with respect to any applicable regulations, including but not limited to privacy, healthcare, employment, or financial regulations.

- You are responsible for complying with all applicable privacy and security regulations including those related to use, collection and handling of any personal data by your app.  This includes complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.

- Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository.  Microsoft’s general trademark guidelines can be found [here](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general.aspx).

- If the app template enables access to any Microsoft Internet-based services (e.g., Office365), use of those services will be subject to the separately-provided terms of use.  In such cases, Microsoft may collect telemetry data related to app template usage and operation.  Use and handling of telemetry data will be performed in accordance with such terms of use.

- Use of this template does not guarantee acceptance of your app to the Teams app store. To make this app available in the Teams app store, you will have to comply with the [submission and validation process](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/appsource/publish), and all associated requirements such as including your own privacy statement and terms of use for your app.


## Feedback 

Thoughts? Questions? Ideas? Share them with us on [Teams UserVoice](https://microsoftteams.uservoice.com/forums/555103-public)  

Please report bugs and other code issues [here](https://github.com/OfficeDev/microsoft-teams-apps-newemployeeonboarding/issues/new). 

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact opencode@microsoft.com with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies.
