import { AppShell } from "../components/layout/AppShell";

const plans = [
  {name:"Free",price:"$0",desc:"For trying EventLens AI.",features:["3 events","10 AI credits / month","1 GB storage","Standard templates"],cta:"Start free"},
  {name:"Creator",price:"$19",desc:"For independent creators.",features:["25 events","250 AI credits / month","50 GB storage","Premium templates"],cta:"Choose Creator",popular:true},
  {name:"Business",price:"$79",desc:"For growing event teams.",features:["200 events","2,500 AI credits / month","500 GB storage","API and priority support"],cta:"Choose Business"},
  {name:"Enterprise",price:"$299",desc:"For agencies and enterprise operations.",features:["10,000 events","25,000 AI credits / month","2 TB storage","Custom domains and enterprise support"],cta:"Contact sales"},
];

export default function Pricing() {
  return <AppShell eyebrow="SIMPLE, FLEXIBLE PLANS" title="Find your perfect plan">
    <div className="pricing-intro"><p>Create more, experiment freely, and upgrade whenever inspiration strikes.</p><div className="billing"><button className="active">Monthly</button><button>Yearly <span>Save 20%</span></button></div></div><div className="pricing-grid">{plans.map(plan=><article className={`price-card ${plan.popular?"popular":""}`} key={plan.name}>{plan.popular&&<span className="popular-tag">MOST POPULAR</span>}<h2>{plan.name}</h2><p>{plan.desc}</p><div className="price"><strong>{plan.price}</strong><span>/ month</span></div><button className={plan.popular?"primary":""}>{plan.cta} <span>→</span></button><div className="divider" /><ul>{plan.features.map(x=><li key={x}><span>✓</span>{x}</li>)}</ul></article>)}</div><p className="pricing-foot">All paid plans include a 7-day free trial. Cancel anytime.</p>
  </AppShell>;
}
