def PROJECT_NAME = "jenkins-unity-test"
def PROJECT_PATH = "/Users/(username)/Desktop/Jenkins_Builds/${PROJECT_NAME}/iOS" //please specify your mac username in the (username) section

pipeline{
    agent{
        label 'iOS_Agent' //please specify your mac agent name
    }

    environment{
        //Keychain information
        KEYCHAIN_PASSWORD = credentials('MAC_MINI_PASSWORD')
        KEYCHAIN_PATH = '/Users/(username)/Library/Keychains/login.keychain-db' //please specify your mac username in the (username) section

        //IPA building configuration
        PLIST_FILE_LOCATION = "/Users/(username)/plist-archive/${PROJECT_NAME}-exporter.plist" //please specify your mac username in the (username) section
        XCODE_PROJECT_PATH = "${PROJECT_PATH}/Unity-iPhone.xcodeproj"
        SCHEME = 'Unity-iPhone'
        CONFIGURATION = 'Release'
        IPA_NAME = "testunityproject.ipa"

        //Signing Information
        TEAM_ID = '' //your personal or company team ID
        PROJECT_IDENTIFIER = '' //your personal bundle ID, Ex:com.defaultcompany.testproject
        PROVISIONING_PROFILE_NAME = '' //your provisioning profile name
        CODE_SIGN_IDENTITY_PLIST = '' //your code sign identity. Ex: Apple Distribution: (company name) (team ID)
        CODE_SIGN_IDENTITY = '' //your code sign identity. Ex: Apple Distribution: (company name) (team ID)
        PROVISIONING_PROFILE_UUID = '' //your provisioning profile UUID. You can edit your provisioning profile file in bbedit or gedit to find the UUID field

        //Test Flight exporting variables
        EXPORTING_USERNAME = '' //your apple account email
        EXPORTING_PASSWORD = credentials('APP_SPECIFIC_PASSWORD')

        //Misc variables
        PATH = "/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:usr/local/sbin"
    }

    stages{
        stage ('Unlock Keychain'){
            steps{
                script{
                    sh '''
                    security unlock-keychain -p ${KEYCHAIN_PASSWORD} ${KEYCHAIN_PATH}
                    '''
                }
            }
        }

        stage('PLIST Configuration file update'){
            steps {
                script {
                    def plistFileExists = fileExists(env.PLIST_FILE_LOCATION)
                    if (!plistFileExists) {
                        def plistContent = """<?xml version="1.0" encoding="utf-8"?> <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
  <dict>
    <key>method</key>
    <string>app-store</string>
    <key>teamID</key>
    <string>${env.TEAM_ID}</string>
    <key>provisioningProfiles</key>
    <dict>
      <key>${env.PROJECT_IDENTIFIER}</key>
      <string>${env.PROVISIONING_PROFILE_NAME}</string>
    </dict>
    <key>signingCertificate</key>
    <string>${env.CODE_SIGN_IDENTITY_PLIST}</string>
    <key>stripSwiftSymbols</key>
    <true/>
    <key>compileBitcode</key>
    <true/>
  </dict>
</plist>"""
                        writeFile file: env.PLIST_FILE_LOCATION, text: plistContent
                        echo "Plist file created at: ${env.PLIST_FILE_LOCATION}"
                    }
                    else {
                        echo "Plist file already exists at: ${env.PLIST_FILE_LOCATION}"
                    }
                }
            }
        }

        stage('Build and Sign IPA with Xcode'){
            steps{
                script{
                    withEnv(["PROJECT_PATH=${PROJECT_PATH}"]){
                        // Changing ownership of files transferred
                        sh """
                        /bin/chmod -R +rwx ${PROJECT_PATH}
                        """

                        // Turn off automatic code signing
                        sh """
                        cd ${XCODE_PROJECT_PATH}
                        sed -i '' 's/CODE_SIGN_STYLE = Automatic;/CODE_SIGN_STYLE = Manual;/g' project.pbxproj
                        """

                        // Clean and build the Xcode project
                        sh """
                        xcodebuild clean -project ${XCODE_PROJECT_PATH} -scheme ${SCHEME} -configuration ${CONFIGURATION}
                        xcodebuild archive -project ${XCODE_PROJECT_PATH} -scheme ${SCHEME} \
                        -configuration ${CONFIGURATION} -archivePath ${PROJECT_PATH}/archive/${SCHEME}.xcarchive \
                        CODE_SIGN_IDENTITY="${CODE_SIGN_IDENTITY}" \
                        PROVISIONING_PROFILE_UUID=${PROVISIONING_PROFILE_UUID} \
                        DEVELOPMENT_TEAM=${TEAM_ID} \
                        OTHER_CODE_SIGN_FLAGS="--keychain ${KEYCHAIN_PATH}"
                        """

                        // Export the IPA from the archive
                        sh """
                        xcodebuild -exportArchive -archivePath ${PROJECT_PATH}/archive/${SCHEME}.xcarchive \
                        -exportOptionsPlist ${PLIST_FILE_LOCATION} \
                        -exportPath ${PROJECT_PATH}/build
                        """
                    }
                }
            }
        }

        stage('Upload to TestFlight') {
            steps {
                script {
                    withEnv(["PROJECT_PATH=${PROJECT_PATH}"]) {
                        echo 'Uploading to TestFlight...'
                        sh '''
                        /usr/bin/xcrun altool --upload-app --type ios --file ${PROJECT_PATH}/build/${IPA_NAME} --username ${EXPORTING_USERNAME} --password ${EXPORTING_PASSWORD}
                        '''
                    }
                }
            }
        }
    }
}