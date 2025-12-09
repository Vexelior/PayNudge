# PayNudge

**PayNudge** is an automated payment reminder system designed to streamline financial management and ensure timely bill payments. Built with C# and .NET 8.0, this lightweight console application integrates seamlessly with Google Sheets to track payment schedules and delivers intelligent email notifications to keep you informed about upcoming, current, and overdue payments.

## Overview

PayNudge eliminates the stress of tracking multiple payment deadlines by automatically monitoring your payment schedule stored in Google Sheets. The application intelligently categorizes payments into three priority levels—overdue, due today, and upcoming (within 3 days)—and sends consolidated email reminders with professionally formatted HTML notifications. Whether you're managing personal bills, rental payments, or business obligations, PayNudge serves as your reliable financial assistant, helping you maintain good payment histories and avoid late fees.

Perfect for individuals, small businesses, and property managers, PayNudge leverages enterprise-grade technologies including the Google Sheets API for data retrieval, MailKit for secure email delivery, and Serilog for comprehensive logging and monitoring. The application's configuration-driven approach makes it easy to customize for various use cases, from personal finance tracking to multi-tenant payment management.

## Features

- Collects payment due dates from a Google Sheet.
- Sends email reminders to users with due dates.
- Configurable email settings.
- Supports multiple recipients.

## Requirements

- .NET 8.0
- Google Sheets API credentials (OAuth 2.0 client ID)
- MailKit library for sending emails

## Setup Instructions

1. Clone the repository:
   ```bash
    git clone https://github.com/Vexelior/PayNudge.git
    cd PayNudge
   ```
2. Install the required NuGet packages:
   ```bash
   dotnet add package Google.Apis.Sheets.v4
   dotnet add package MailKit
   ```
3. Create a Google Cloud project and enable the Google Sheets API.
4. Download the OAuth 2.0 credentials JSON file and place it in the project directory.
5. Update the `Program.cs` file with your Google Sheet ID and email settings.
6. Build and run the application:
   ```bash
   dotnet run
   ```

## Usage

- The program will read the specified Google Sheet for payment due dates.
- It will send email reminders to the specified recipients based on the due dates found in the sheet.
- Ensure that the Google Sheet is shared with the service account email if using OAuth 2.0.

## Configuration

- Update the `Program.cs` file with your Google Sheet ID and email settings.
- Create a `appsettings.json` file in the project directory with your OAuth 2.0 credentials in this format:
  ```json
  {
    "EmailSettings": {
      "Sender": "",
      "AppPassword": "",
      "Recipients": ["", ""]
    }
  }
  ```

## Troubleshooting

- Ensure that the Google Sheets API is enabled in your Google Cloud project.
- Check that the OAuth 2.0 credentials JSON file is correctly placed in the build directory with all DLL dependencies.
- Verify that the Google Sheet ID is correct and that the sheet is accessible.
- If you encounter issues with sending emails, verify that the email settings in `appsettings.json` are correct.
- If you receive authentication errors, ensure that the OAuth 2.0 credentials are valid and that the service account has access to the Google Sheet.

## License

This project is licensed under the Apache License 2.0. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please submit a pull request or open an issue if you find a bug or have a feature request.

## Contact

For any questions or feedback, please open an issue on the GitHub repository or contact the project maintainer.
