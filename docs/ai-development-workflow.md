# AI Development Workflow

This repository will be built with an AI-assisted development workflow.

## Pillars

### 1. Spec-First Development

Before implementation, each meaningful feature should define:

- user story
- acceptance criteria
- OpenAPI changes
- request and response contracts
- data model changes
- edge cases
- tests required

AI implements against the spec. Review checks whether the result matches the spec.

### 2. Phase-Specific AI Usage

Different phases need different prompts and model choices.

- Requirement understanding: AI as clarifier and challenger
- Design and spec: AI as architecture reviewer
- Task breakdown: AI as planner
- Implementation: AI as implementer
- Review and verification: AI as reviewer/tester
- Refactor and hardening: AI as senior engineering assistant
- Documentation: AI as technical writer

### 3. Skills and Reusable Prompts

When a prompt or workflow works well, extract it into a reusable skill.

Each skill should document:

- purpose
- required inputs
- expected output
- examples
- failure modes
- recommended model level

### 4. Monitoring AI Work

Track AI-assisted work across:

- quality: did the output match the spec?
- efficiency: how many iterations were needed?
- consumption: which model was used, and what was the estimated cost?

The project will eventually include an AI monitoring feature for this.

## Model Selection Policy

Use cheaper/faster models for:

- simple boilerplate
- formatting
- README cleanup
- issue template generation
- small CRUD implementation from a clear spec

Use stronger models for:

- architecture decisions
- complex debugging
- security review
- concurrency problems
- performance optimization
- final review before merge

