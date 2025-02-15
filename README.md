**Pipeline Tester - Ensuring CI/CD Pipeline Integrity**

Overview

This project validates that the Azure DevOps pipeline configuration (azure-pipelines.yml) correctly aligns with the dependencies specified in PipelineTester.sln. The approach ensures:

Consistency between CI/CD pipeline and project dependencies

Automatic detection of missing or extra references

Identification of unwanted or vulnerable package references

Reliability in deployment by maintaining integrity between project structure and pipeline triggers

The methodology follows the principles outlined in this [article](https://www.linkedin.com/pulse/unit-testing-project-references-cicd-pipelines-javid-gahramanov-ngmgf/?trackingId=Et1bntgPT7EOMOpa1TVeMA%3D%3D), which highlights the importance of verifying project references in CI/CD pipelines.

**Key Features**

Automated Pipeline Verification: Ensures that azure-pipelines.yml only includes valid dependencies.

Project Dependency Analysis: Uses MSBuild to inspect solution files and project references.

YAML Configuration Parsing: Utilizes YamlDotNet to extract and validate trigger paths.

Security Check for Vulnerable Packages: Detects well-known vulnerable packages and ensures they are not referenced.

**Used Libraries**

[ _Microsoft.Build_](https://www.nuget.org/packages/Microsoft.Build) , [ _YamlDotNet_](https://github.com/aaubry/YamlDotNet)
