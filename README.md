# Veteran Analytics System

Veteran Analytics System is a .NET web application meant to provide a dashboard of analytics for veteran-assistance program data imported from Google Forms and Ragic.

## Local Setup

1. In your user secrets, you will need the Ragic Api Key
    ```
    {
      "RagicAPIKey": "[YourApiKey]"
    }
    ```
1. Publish the database to your local sql server instance. The default database name is `VeteranAnalytics`