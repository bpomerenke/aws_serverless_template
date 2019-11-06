//
//  ViewController.swift
//  dummy_iot
//
//  Created by Benjamin Pomerenke on 9/28/19.
//  Copyright © 2019 Benjamin Pomerenke. All rights reserved.
//

import UIKit
import CocoaMQTT
struct Message: Decodable {
    let sender: String
    let msgType: String
    let msgText: String
    let msgTime: Double
}
class ViewController: UIViewController {
    let clientID = "test02"
    let defaultHost = "ab5bhz2ubggz4-ats.iot.us-east-2.amazonaws.com"
    var mqtt: CocoaMQTT?
    
    @IBOutlet weak var messageTable: UITableView!
    var messages: [String] = []
    
    @IBOutlet weak var messageText: UITextField!
    @IBOutlet weak var sendButton: UIButton!
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        messageTable.delegate = self
        messageTable.dataSource = self
        messageText.delegate = self
    }

    @IBAction func sendMessage(_ sender: Any) {
        print("sending...")
        let now = Date().timeIntervalSince1970;
        let message = "{\"sender\": \"\(clientID)\", \"msgType\": \"Message\", \"msgText\": \"\(messageText.text!)\", \"msgTime\": \(now)}"
        
        mqtt!.publish("CHAT/Messages", withString: message, qos: .qos0)
    }
    @IBAction func connect(_ sender: Any) {
        print("connecting...")
        selfSignedSSLSetting()
    }
    
    func selfSignedSSLSetting() {
        mqtt = CocoaMQTT(clientID: clientID, host: defaultHost, port: 8883)
        mqtt!.username = ""
        mqtt!.password = ""
        mqtt!.willMessage = CocoaMQTTWill(topic: "/will", message: "dieout")
        mqtt!.keepAlive = 60
        mqtt!.delegate = self
        mqtt!.enableSSL = true
        mqtt!.allowUntrustCACertificate = true
        
        let clientCertArray = getClientCertFromP12File(certName: clientID, certPassword: "123")
        
        var sslSettings: [String: NSObject] = [:]
        sslSettings[kCFStreamSSLCertificates as String] = clientCertArray
        
        mqtt!.sslSettings = sslSettings
        
        let connecting = mqtt!.connect()
        print("connecting: \(connecting)")
    }
    
    func getClientCertFromP12File(certName: String, certPassword: String) -> CFArray? {
        // get p12 file path
        let resourcePath = Bundle.main.path(forResource: certName, ofType: "p12")
        
        guard let filePath = resourcePath, let p12Data = NSData(contentsOfFile: filePath) else {
            print("Failed to open the certificate file: \(certName).p12")
            return nil
        }
        
        // create key dictionary for reading p12 file
        let key = kSecImportExportPassphrase as String
        let options : NSDictionary = [key: certPassword]
        
        var items : CFArray?
        let securityError = SecPKCS12Import(p12Data, options, &items)
        
        guard securityError == errSecSuccess else {
            if securityError == errSecAuthFailed {
                print("ERROR: SecPKCS12Import returned errSecAuthFailed. Incorrect password?")
            } else {
                print("Failed to open the certificate file: \(certName).p12")
            }
            return nil
        }
        
        guard let theArray = items, CFArrayGetCount(theArray) > 0 else {
            return nil
        }
        let dictionary = (theArray as NSArray).object(at: 0)
        guard let identity = (dictionary as AnyObject).value(forKey: kSecImportItemIdentity as String) else {
            return nil
        }
        
        let certArray = [identity] as CFArray
        
        return certArray
    }

}

extension ViewController: UITextFieldDelegate {
     /**
      * Called when 'return' key pressed. return NO to ignore.
      */
    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
         messageText.resignFirstResponder()
         return true
     }
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.view.endEditing(true)
    }

}

extension ViewController: UITableViewDelegate, UITableViewDataSource {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.messages.count;
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        if let cell = tableView.dequeueReusableCell(withIdentifier: "MessageCell") {
            // Set text of textLabel.
            // ... Use indexPath.item to get the current row index.
            if let label = cell.textLabel {
                label.text = messages[indexPath.item]
            }
            // Return cell.
            return cell
        }

        // Return empty cell.
        return UITableViewCell()
    }
    
    
}

// From: https://github.com/emqx/CocoaMQTT

extension ViewController: CocoaMQTTDelegate {
    // Optional ssl CocoaMQTTDelegate
    func mqtt(_ mqtt: CocoaMQTT, didReceive trust: SecTrust, completionHandler: @escaping (Bool) -> Void) {
        print("trust: \(trust)")
        /// Validate the server certificate
        ///
        /// Some custom validation...
        ///
        /// if validatePassed {
        ///     completionHandler(true)
        /// } else {
        ///     completionHandler(false)
        /// }
        completionHandler(true)
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didConnectAck ack: CocoaMQTTConnAck) {
        print("ack: \(ack)")
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didStateChangeTo state: CocoaMQTTConnState) {
        print("new state: \(state)")
        if state.description == "connected" {
            self.mqtt!.subscribe("CHAT/Messages")
            self.sendButton!.isEnabled = true
        }
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didPublishMessage message: CocoaMQTTMessage, id: UInt16) {
        print("publish message: \(message.string!.description), id: \(id)")
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didPublishAck id: UInt16) {
        print("id: \(id)")
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didReceiveMessage message: CocoaMQTTMessage, id: UInt16 ) {
        print("recv'd message: \(message.string!.description), id: \(id)")
        do {
            let result = try JSONDecoder().decode(Message.self, from: message.string!.data(using: .utf8)!)

            self.messages.append("\(result.sender): \(result.msgText)")
            self.messageTable.reloadData()
            print("loaded \(result.msgText)")
        } catch {
            print("failed decoding")
        }
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didSubscribeTopic topics: [String]) {
        print("topics: \(topics)")
    }
    
    func mqtt(_ mqtt: CocoaMQTT, didUnsubscribeTopic topic: String) {
        print("topic: \(topic)")
    }
    
    func mqttDidPing(_ mqtt: CocoaMQTT) {
        print()
    }
    
    func mqttDidReceivePong(_ mqtt: CocoaMQTT) {
        print()
    }

    func mqttDidDisconnect(_ mqtt: CocoaMQTT, withError err: Error?) {
        print("err: \(err.debugDescription)")
    }
}

