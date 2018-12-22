using System;

namespace JavaScript
{

	/// <summary>
	/// Represents an implementation of the Firebug API using the ScriptEngine OnLog method.
	/// </summary>
	internal class ConsoleOutput : IconsoleOutput
	{
		private int currentIndentation;
		private int indentationDelta = 4;

		/// <summary>
		/// Gets or sets the number of spaces to output before writing any text to the console.
		/// </summary>
		public int CurrentIndentation
		{
			get { return this.currentIndentation; }
			set
			{
				if (value < 0 || value > 40)
					throw new ArgumentOutOfRangeException("value");
				this.currentIndentation = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of spaces to add to the identation when group() is called.
		/// </summary>
		public int IndentationDelta
		{
			get { return this.indentationDelta; }
			set
			{
				if (value < 0 || value > 40)
					throw new ArgumentOutOfRangeException("value");
				this.indentationDelta = value;
			}
		}

		/// <summary>
		/// Logs a message to the console.
		/// </summary>
		/// <param name="style"> A style which influences the icon and text color. </param>
		/// <param name="objects"> The objects to output to the console. These can be strings or
		/// ObjectInstances. </param>
		public void Log(consoleMessageStyle style, object[] objects)
		{
			// Convert the objects to a string.
			var message = new System.Text.StringBuilder();
			for (int i = 0; i < objects.Length; i++)
			{
				if(i!=0){
					message.Append(' ');
				}
				
				message.Append(objects[i].ToString());
			}

			// Output the message to the console.
			UnityEngine.Debug.Log(message.ToString());
			
		}

		/// <summary>
		/// Clears the console.
		/// </summary>
		public void Clear()
		{
		}

		/// <summary>
		/// Starts grouping messages together.
		/// </summary>
		/// <param name="title"> The title for the group. </param>
		/// <param name="initiallyCollapsed"> <c>true</c> if subsequent messages should be hidden by default. </param>
		public void StartGroup(string title, bool initiallyCollapsed)
		{
			Log(consoleMessageStyle.Regular, new object[] { title });
			this.CurrentIndentation = Math.Min(this.CurrentIndentation + this.IndentationDelta, 40);
		}

		/// <summary>
		/// Ends the most recently started group.
		/// </summary>
		public void EndGroup()
		{
			this.CurrentIndentation = Math.Max(this.CurrentIndentation - this.IndentationDelta, 0);
		}
	}
}
