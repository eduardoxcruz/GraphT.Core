using GraphT.Model.ValueObjects;

namespace GraphT.Model.Entities;

public class TodoItem
{
	public Guid Id { get; }
	public string Name { get; set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Relevance Relevance => new(IsFun, IsProductive);
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	
	public TodoItem(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty");
		
		Id = Guid.NewGuid();
		Name = name;
		Complexity = Complexity.Indefinite;
		Priority = Priority.Distraction;
	}
}
