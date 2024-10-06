def PROJECT_NAME = "jenkins-unity-test"
def CUSTOM_WORKSPACE = "C:\\Jenkins\\Unity_Projects\\${PROJECT_NAME}"
def UNITY_VERSION = "2022.3.46f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"

pipeline{
    environment{
        PROJECT_PATH = "${CUSTOM_WORKSPACE}\\${PROJECT_NAME}"
        NEXUS_IP_ADDRESS = "" //Your full Nexus IP address+port. Example: http://192.168.1.200:8081
        NEXUS_USERNAME = "" //Your Nexus username
        NEXUS_PASSWORD = credentials('NEXUS_PASSWORD')
        MAC_PASSWORD = credentials('MAC_PASSWORD')

        GOOGLE_PLAY_API_JSON_LOCATION = credentials('GOOGLE_PLAY_API_JSON_LOCATION')
        TEST_PROJECT_KEYSTORE_FILE = credentials('TEST_PROJECT_KEYSTORE_FILE')
        KEYSTORE_PASS = credentials('KEYSTORE_PASS')
        ALIAS_NAME = credentials('ALIAS_NAME')
        ALIAS_PASS = credentials('ALIAS_PASS')
        BUNDLE_ID = "" //Your bundle ID. Ex: com.defaultcompany.test
        MAC_IP_ADDRESS = "" //Your mac IP address
        MAC_USERNAME = "" //Your mac username
    }

    agent{
        label{
            label ""
            customWorkspace "${CUSTOM_WORKSPACE}"
        }
    }

    stages{
        stage('Build Windows'){
            when{expression {BUILD_WINDOWS == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWindows -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy Windows'){
            when{expression {DEPLOY_WINDOWS == 'true'}}
            steps{
                script{
                    def buildDate = new Date().format("yyyyMMdd_HHmm")
                    env.ARTIFACT_NAME = "Windows_Build_${buildDate}.zip"

                    bat '''
                    curl -u %NEXUS_USERNAME%:%NEXUS_PASSWORD% --upload-file %PROJECT_PATH%/Builds/Windows.zip %NEXUS_IP_ADDRESS%/repository/jenkins_unity_test/Windows_Builds/%ARTIFACT_NAME%
                    '''
                }
            }
        }
        stage('Build WebGL'){
            when{expression {BUILD_WEBGL == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWebGL -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy WebGL'){
            when{expression {DEPLOY_WEBGL == 'true'}}
            steps{
                script{
                    def buildDate = new Date().format("yyyyMMdd_HHmm")
                    env.ARTIFACT_NAME = "WebGL_Build_${buildDate}.zip"

                    bat '''
                    curl -u %NEXUS_USERNAME%:%NEXUS_PASSWORD% --upload-file %PROJECT_PATH%/Builds/WebGL.zip %NEXUS_IP_ADDRESS%/repository/jenkins_unity_test/WebGL_Builds/%ARTIFACT_NAME%
                    '''
                }
            }
        }

        stage('Build Android APK'){
            when{expression {BUILD_ANDROID_APK == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildAndroid -buildType APK -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy Android APK - Nexus'){
            when{expression {DEPLOY_ANDROID_APK == 'true'}}
            steps{
                script{
                    def buildDate = new Date().format("yyyyMMdd_HHmm")
                    env.ARTIFACT_NAME = "Android_Build_${buildDate}.apk"

                    bat '''
                    curl -u %NEXUS_USERNAME%:%NEXUS_PASSWORD% --upload-file %PROJECT_PATH%/Builds/AndroidAPK/TestGame.apk %NEXUS_IP_ADDRESS%/repository/jenkins_unity_test/AndroidAPK_Builds/%ARTIFACT_NAME%
                    '''
                }
            }
        }

        stage('Build Android AAB'){
            when{expression {BUILD_ANDROID_AAB == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildAndroid -buildType AAB -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy Android AAB - Nexus'){
            when{expression {DEPLOY_ANDROID_AAB == 'true'}}
            steps{
                script{
                    def buildDate = new Date().format("yyyyMMdd_HHmm")
                    env.ARTIFACT_NAME = "Android_Build_${buildDate}.aab"

                    bat '''
                    curl -u %NEXUS_USERNAME%:%NEXUS_PASSWORD% --upload-file %PROJECT_PATH%/Builds/AndroidAAB/TestGame.aab %NEXUS_IP_ADDRESS%/repository/jenkins_unity_test/AndroidAAB_Builds/%ARTIFACT_NAME%
                    '''
                }
            }
        }

        stage('Deploy Android - Google Play'){
            when{expression {DEPLOY_ANDROID_AAB == 'true'}}
            steps{
                script{
                    bat '''
                    fastlane supply ^
                    --track internal ^
                    --release_status "draft" ^
                    --aab "%PROJECT_PATH%/Builds/AndroidAAB/TestGame.aab" ^
                    --json_key "%GOOGLE_PLAY_API_JSON_LOCATION%" ^
                    --package_name "%BUNDLE_ID%" ^
                    --version_name "V%BUILD_NUMBER% - Test Project"
                    '''
                }
            }
        }

        stage('Build iOS'){
            when{expression {BUILD_IOS == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildIOS -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy iOS Nexus'){
            when{expression {DEPLOY_IOS_NEXUS == 'true'}}
            steps{
                script{
                    def buildDate = new Date().format("yyyyMMdd_HHmm")
                    env.ARTIFACT_NAME = "iOS_Build_${buildDate}.zip"

                    bat '''
                    curl -u %NEXUS_USERNAME%:%NEXUS_PASSWORD% --upload-file %PROJECT_PATH%/Builds/iOS.zip %NEXUS_IP_ADDRESS%/repository/jenkins_unity_test/iOS_Builds/%ARTIFACT_NAME%
                    '''
                }
            }
        }

        stage('Deploy iOS Mac'){
            when{expression {DEPLOY_IOS_MAC == 'true'}}
            steps{
                script{
                    env.PROJECT_NAME = PROJECT_NAME

                    powershell '''
                    net use \\\\%MAC_IP_ADDRESS% /user:%MAC_USERNAME% $env:MAC_PASSWORD
                    Remove-Item -Path \\\\%MAC_IP_ADDRESS%\\%MAC_USERNAME%\\Desktop\\Jenkins_Builds\\$env:PROJECT_NAME -Recurse -Force -ErrorAction Ignore
                    New-Item -ItemType directory -Path \\\\%MAC_IP_ADDRESS%\\%MAC_USERNAME%\\Desktop\\Jenkins_Builds\\$env:PROJECT_NAME -Force
                    Copy-Item -Path "$env:PROJECT_PATH\\Builds\\iOS" -Destination \\\\%MAC_IP_ADDRESS%\\%MAC_USERNAME%\\Desktop\\Jenkins_Builds\\$env:PROJECT_NAME -Recurse -Force
                    net use \\\\%MAC_IP_ADDRESS% /delete
                    '''
                }
            }
        }
    }
}