namespace ConsoleApplication1.Code.Common
{
    
    // 节点类(存放int值)
    public class ValueNode{
        public int Value;
        public ValueNode NextNode;
        // 无参构造
        public ValueNode() {
        }
        // 有参构造
        public ValueNode(int value) {
            this.Value = value;
        }

        public ValueNode(int value, ValueNode nextNode) {
            this.Value = value;
            this.NextNode = nextNode;
        }
    }
}