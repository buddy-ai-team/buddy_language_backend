import azure.functions as func
import io
import logging
import pyogg

app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)


@app.route(route="convert_ogg_opus_to_raw_pcm")
def convert_ogg_opus_to_raw_pcm(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    try:
        # Read the OGG file from the request
        ogg_file = req.get_body()

        # Convert the OGG file to raw PCM using PyOgg
        opus_buffer = io.BytesIO(ogg_file)
        pcm_buffer = io.BytesIO()
        ogg_stream = pyogg.OpusFile(opus_buffer)
        
        # Check for mono channel and resample if needed
        if ogg_stream.channels == 1:
            pcm_data = ogg_stream.as_array()
        else:
            # Extract only the first channel for mono
            pcm_data = ogg_stream.as_array()[:, 0]

        # Resample to 16kHz if necessary
        if ogg_stream.frequency != 16000:
            from scipy.signal import resample
            num_samples = int(len(pcm_data) * 16000 / ogg_stream.frequency)
            pcm_data = resample(pcm_data, num_samples)

        # Ensure 16-bit depth
        pcm_data = pcm_data.astype('int16')

        # Write PCM data to buffer
        pcm_buffer.write(pcm_data.tobytes())

        # Return the raw PCM data
        return func.HttpResponse(pcm_buffer.getvalue(), mimetype='audio/pcm')

    except Exception as e:
        logging.error(f'Error: {str(e)}')
        return func.HttpResponse(f"Failed to convert the file. Error: {str(e)}", status_code=500)
