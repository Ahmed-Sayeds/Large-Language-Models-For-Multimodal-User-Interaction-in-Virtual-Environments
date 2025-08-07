using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You will provide me with a segment of natural language text describing actions and manipulations within a virtual environment. My task will be to analyze this text and perform the following:\r\n\r\nClass: I will identify the objective of the text. Using the provided list, I will determine which item best matches the objective, and return the index of that item as a string. The list is as follows:\r\nvbnet\r\nCopy code\r\n[\r\n1 - Rotate an object.\r\n2 - Resize an object.\r\n3 - Create an object.\r\n4 - Delete an object.\r\n5 - Select an object.\r\n6 - Add material to an object.\r\n7 - Add a script to an object.\r\n8 - Add a component to an object, such as a rigidbody.\r\n9 - Move an object.\r\n10 - Move to a location.\r\n11 - Teleport to a location.\r\n]\r\nObject: I will extract any mentioned objects from the text. These objects can be referred to with single or multi-word names, or non-specific pronouns like \"that\", within the virtual environment context. I will return a list of these object names.\r\nValues: I will extract any values related to an object. If the text contains one or more vector3 values, I will return these as a sublist within the values list. If any values in a vector3 coordinate are missing, I will fill in those values with an empty string \"\". Non-vector values or the word \"there\" or any similar terms will be added directly to the values list. If the text contains words like \"greater\", \"smaller\", \"multiply\", \"divide\", or \"power\", I will convert them to \"+\", \"-\", \"*\", \"/\", and \"^\" respectively and place the symbol before the value. I will return these as a list of strings or list of lists.\r\nI will compile the results into a JSON format and return it. The returned JSON should have the structure:\r\njson\r\nCopy code\r\n{\r\n\"index\": \"<index>\",\r\n\"objects\": [\"<object1>\", \"<object2>\", ...],\r\n\"values\": [[\"<value1>\", \"<value2>\", \"<value3>\"], ..., \"<valueN>\"],\r\n}\r\nIf the text does not contain any objects or values, I will return empty lists in the respective places.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }
        
    }
}
