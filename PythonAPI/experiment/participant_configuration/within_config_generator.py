import os

# Define conditions
n_back_levels = ["One", "Two"]
interruption_methods = ["Immediate", "Negotiated", "Scheduled"]

# Define the conditions mapping based on the given table
condition_mapping = [
    ("One", "Immediate"),   # I1
    ("Two", "Immediate"),   # I2
    ("One", "Negotiated"),  # N1
    ("Two", "Negotiated"),  # N2
    ("One", "Scheduled"),   # S1
    ("Two", "Scheduled"),   # S2
]

# Define the order of conditions for each subject based on the table
latin_square_order = [
    [0, 1, 2, 3, 4, 5],  # Order for Subject 1
    [1, 2, 3, 4, 5, 0],  # Order for Subject 2
    [2, 3, 4, 5, 0, 1],  # Order for Subject 3
    [3, 4, 5, 0, 1, 2],  # Order for Subject 4
    [4, 5, 0, 1, 2, 3],  # Order for Subject 5
    [5, 0, 1, 2, 3, 4],  # Order for Subject 6
]

# Participant template
template = """[General]
ParticipantID="{participant_id}"
LogPerformance="True"
CurrentBlock="Block0Trial1"

[OneBackTestTrial]
SkipSR="True"
NDRTTaskType="VisualNBackTask"
TaskSetting="One"
InterruptionMethod="Immediate"

[TwoBackTestTrial]
SkipSR="True"
NDRTTaskType="VisualNBackTask"
TaskSetting="Two"
InterruptionMethod="Immediate"

{blocks}
"""

# Block template
block_template = """[Block1Trial{trial_num}]
SkipSR="False"
NDRTTaskType="VisualNBackTask"
TaskSetting="{n_back_level}"
InterruptionMethod="{interruption_method}"
"""

# Create config_files directory if it doesn't exist
if not os.path.exists('config_files'):
    os.makedirs('config_files')

# Generate configuration files
for i in range(18):
    participant_id = f"P{i+1:02}"
    block_entries = []

    # Determine the correct order of conditions for this participant
    order_index = i % len(latin_square_order)
    order = latin_square_order[order_index]

    for trial_num, condition_index in enumerate(order, start=1):
        n_back_level, interruption_method = condition_mapping[condition_index]
        block_entry = block_template.format(trial_num=trial_num, n_back_level=n_back_level, interruption_method=interruption_method)
        block_entries.append(block_entry)
    
    blocks = "\n".join(block_entries)
    config_content = template.format(participant_id=participant_id, blocks=blocks)
    
    # Write to file in the config_files directory with the specified naming convention
    with open(f"config_files/ExperimentConfig_{participant_id}.ini", "w") as config_file:
        config_file.write(config_content)