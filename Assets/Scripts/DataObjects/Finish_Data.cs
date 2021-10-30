//Wrapper object for saving the data from a finish
//Exists becuase Monobehavior inheritence doesn't allow constructors
public class Finish_Data
{
    //Stores the selected cards
    public Card[] selectedCards;

    //Constructor
    public Finish_Data(Card[] _selectedCards)
    {
        selectedCards = _selectedCards;
    }
}
