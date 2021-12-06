namespace Cdm.Figma.UIToolkit
{
    public interface IScriptGenerator
    {
        GeneratedScript Generate(NodeElement page);
    }
    
    public class GeneratedScript
    {
        /// <summary>
        /// Class name of the generated script.
        /// </summary>
        public string name { get; }
        
        /// <summary>
        /// Contents of the script.
        /// </summary>
        public string contents { get; }

        public GeneratedScript(string name, string contents)
        {
            this.name = name;
            this.contents = contents;
        }
    }
}