import{ useState } from "react";
import{ api } from "../services/api";

type Line = {
	accountId: number;
	debit: number;
	credit: number;
};

export default function CreateJournalEntry(){
	const[date, setDate]= useState("");
	const[lines, setLines] = useState<Line[]>([
	{ accountId: 1, debit: 0, credit: 0},
	{ accountId: 2, debit: 0, credit: 0}
]);

const addLine = () => {
	setLines([...lines,{accountId: 0, debit: 0, credit: 0}]);
};

const updateLine = (index: number, field: keyof Line, value: number) => {
	const updated = [...lines];
	updated[index][field] = value;
	setLines(updated);
};

const totalDebit = lines.reduce((sum,l) => sum + l.debit, 0);
const totalCredit = lines.reduce((sum,l) => sum + l.credit, 0);

const submit = async() => {
	
	if(totalDebit !== totalCredit){
		alert("Entry is not balanced!");
		return;
	}
	
	await api.post("/journalentries",{
		date,
		lines
	});
	
	alert("Saved!");
};

return (
	<div>
		<h2>Create Journal Entry</h2>
		
		<input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
		
		{lines.map((line,i) => (
			<div key={i}>
				<input type="number" placeholder="AccountId" value={line.accountId} onChange={(e) => 
					updateLine(i, "accountId", Number(e.target.value))
				}
				/>
				
				<input type="number" placeholder="Debit" value={line.debit} onChange={(e) => 
					updateLine(i, "debit", Number(e.target.value))
				}
				/>	
				
				<input type="number" placeholder="Credit" value={line.credit} onChange={(e) => 
					updateLine(i, "credit", Number(e.target.value))
				}
				/>
			</div>
		))}
		
		<button onClick={submit}>Save</button>
	</div>
	);
}
		